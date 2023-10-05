namespace MyThreadPool;

using System.Collections.Concurrent;
using Utils;

/// <summary>
/// A thread pool that executes tasks in parallel.
/// </summary>
public class MyThreadPool
{
    private MyThread[] threads;
    private ConcurrentQueue<IExecutable> tasks = new();
    private Semaphore tasksAmount = new(0, int.MaxValue);
    private volatile bool keepExecuting = true;
    private ManualResetEvent queueIsEmpty = new(false);
    private volatile int tasksToEnqueue = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyThreadPool"/> class with the specified amount of threads.
    /// </summary>
    /// <param name="threadsAmount">The amount of threads to create.</param>
    public MyThreadPool(int threadsAmount)
    {
        this.threads = Enumerable
            .Range(0, threadsAmount)
            .Select(_ => new MyThread(this.ProvideTask))
            .ToArray();
    }

    /// <summary>
    /// Represents a task that is executed by a thread pool.
    /// </summary>
    private interface IExecutable
    {
        /// <summary>
        /// Executes the task.
        /// </summary>
        public void Execute();
    }

    /// <summary>
    /// Submits a task to the thread pool.
    /// </summary>
    /// <typeparam name="T">The type of the task's result.</typeparam>
    /// <param name="task">The task to submit.</param>
    /// <returns>A task that represents the submitted task.</returns>
    /// <throws><see cref="InvalidOperationException"/> if the thread pool is shut down.</throws>
    public IMyTask<T> Submit<T>(Func<T> task)
    {
        lock (this.tasks)
        {
            if (!this.keepExecuting)
            {
                throw new InvalidOperationException("The thread pool is shut down.");
            }

            Task<T> myTask = new(this, task);
            this.tasks.Enqueue(myTask);
            this.tasksAmount.Release();
            return myTask;
        }
    }

    /// <summary>
    /// Submits a task to the thread pool.
    /// </summary>
    /// <param name="task">The task to submit.</param>
    /// <returns>A task that represents the submitted task.</returns>
    /// <throws><see cref="InvalidOperationException"/> if the thread pool is shut down.</throws>
    public IMyTask Submit(Action task)
    {
        Task myTask;
        lock (this.tasks)
        {
            if (!this.keepExecuting)
            {
                throw new InvalidOperationException("The thread pool is shut down.");
            }

            myTask = new(this, task);
            this.tasks.Enqueue(myTask);
        }

        this.tasksAmount.Release();
        return myTask;
    }

    /// <summary>
    /// Shuts down the thread pool. The method blocks until all tasks are completed and all threads are shut down.
    /// </summary>
    public void Shutdown()
    {
        this.keepExecuting = false;
        this.UpdateQueueIsEmpty();
        if (this.tasksToEnqueue != 0)
        {
            this.queueIsEmpty.WaitOne();
        }

        foreach (var thread in this.threads)
        {
            thread.Shutdown();
            this.tasks.Enqueue(new EmptyExecutable());
        }

        this.tasksAmount.Release(this.threads.Length);

        foreach (var thread in this.threads)
        {
            thread.Wait();
        }
    }

    /// <summary>
    /// Shuts down the thread pool before it is garbage collected.
    /// </summary>
    public void Dispose() => this.Shutdown();

    private void Submit(IExecutable task)
    {
        this.tasks.Enqueue(task);
        this.tasksAmount.Release();
        Interlocked.Decrement(ref this.tasksToEnqueue);
    }

    private Action ProvideTask()
    {
        this.tasksAmount.WaitOne();
        this.tasks.TryDequeue(out IExecutable? task);
        this.UpdateQueueIsEmpty();
        return task!.Execute;
    }

    private void UpdateQueueIsEmpty()
    {
        lock (this.tasks)
        {
            if (!this.keepExecuting && this.tasks.IsEmpty && this.tasksToEnqueue == 0)
            {
                this.queueIsEmpty.Set();
            }
        }
    }

    private class EmptyExecutable : IExecutable
    {
        /// <summary>
        /// Does literally nothing.
        /// </summary>
        public void Execute() { }
    }

    private class Task : Task<Monostate>, IMyTask
    {
        public Task(MyThreadPool threadPool, Action task)
            : base(
                threadPool,
                () =>
                {
                    task();
                    return Monostate.Instance;
                }
            )
        { }

        public IMyTask<TNew> ContinueWith<TNew>(Func<TNew> continuation) =>
            this.ContinueWith((Monostate _) => continuation());

        public IMyTask ContinueWith(Action continuation) =>
            this.ContinueWith((Monostate _) => continuation());
    }

    private class Task<T> : IMyTask<T>, IExecutable
    {
        private MyThreadPool threadPool;
        private bool isCompleted = false;
        private Option<Result<T, Exception>> result = Option<Result<T, Exception>>.None;
        private ManualResetEvent executeCompleted = new(false);
        private Option<Func<T>> task;
        private List<IExecutable> continuations = new();

        public Task(MyThreadPool threadPool, Func<T> task)
        {
            this.task = task;
            this.threadPool = threadPool;
        }

        public bool IsCompleted => this.isCompleted;

        /// <inheritdoc cref="IMyTask{T}.Result"/>
        public T Result
        {
            get
            {
                if (this.isCompleted)
                {
                    return this.result.Unwrap().UnwrapOrElse(e => throw new AggregateException(e));
                }

                this.executeCompleted.WaitOne();
                return Volatile
                    .Read(ref this.result)
                    .Unwrap()
                    .UnwrapOrElse(e => throw new AggregateException(e));
            }
        }

        public void Execute()
        {
            if (!this.task.IsSome())
            {
                return;
            }

            this.result = Try<Exception>.Call(this.task.Unwrap());

            lock (this.result)
            {
                Volatile.Write(ref this.isCompleted, true);
                this.executeCompleted.Set();
                foreach (var continuation in this.continuations)
                {
                    this.threadPool.Submit(continuation);
                }
            }
        }

        public void Wait() => this.executeCompleted.WaitOne();

        public IMyTask<TNew> ContinueWith<TNew>(Func<T, TNew> continuation)
        {
            this.IncrementTaskAmount();
            Task<TNew> newTask = new(this.threadPool, () => continuation(this.Result));
            this.SubmitContinuation(newTask);
            return newTask;
        }

        public IMyTask ContinueWith(Action<T> continuation)
        {
            this.IncrementTaskAmount();
            Task newTask = new(this.threadPool, () => continuation(this.Result));
            this.SubmitContinuation(newTask);
            return newTask;
        }

        private void IncrementTaskAmount()
        {
            Interlocked.Increment(ref this.threadPool.tasksToEnqueue);
            if (!this.threadPool.keepExecuting)
            {
                Interlocked.Decrement(ref this.threadPool.tasksToEnqueue);
                this.threadPool.UpdateQueueIsEmpty();
                throw new InvalidOperationException("The thread pool is shut down.");
            }
        }

        private void SubmitContinuation(IExecutable task)
        {
            lock (this.result)
            {
                if (Volatile.Read(ref this.isCompleted))
                {
                    this.threadPool.Submit(task);
                }
                else
                {
                    this.continuations.Add(task);
                }
            }
        }
    }
}
