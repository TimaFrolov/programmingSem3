using System.Collections.Concurrent;
using Utils;

namespace MyThreadPool;

public class MyThreadPool
{
    private interface Executable
    {
        public void Execute();
    }

    private class EmptyExecutable : Executable
    {
        public void Execute() { }
    }

    private MyThread[] threads;
    private ConcurrentQueue<Executable> tasks = new();
    private Semaphore tasksAmount = new(0, int.MaxValue);
    private volatile bool keepExecuting = true;
    private ManualResetEvent queueIsEmpty = new(false);
    private volatile int tasksToEnqueue = 0;

    public MyThreadPool(int threadsAmount)
    {
        this.threads = Enumerable
            .Range(0, threadsAmount)
            .Select(_ => new MyThread(this.ProvideTask))
            .ToArray();
    }

    private Action ProvideTask()
    {
        this.tasksAmount.WaitOne();
        this.tasks.TryDequeue(out Executable? task);
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

    private void Submit(Executable task)
    {
        this.tasks.Enqueue(task);
        this.tasksAmount.Release();
        Interlocked.Decrement(ref this.tasksToEnqueue);
    }

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

    public void Dispose()
    {
        this.Shutdown();
        this.queueIsEmpty.Dispose();
        this.tasksAmount.Dispose();
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
            ) { }

        public IMyTask<TNew> ContinueWith<TNew>(Func<TNew> continuation) =>
            this.ContinueWith((Monostate _) => continuation());
    }

    private class Task<T> : IMyTask<T>, Executable
    {
        private MyThreadPool threadPool;

        private bool isCompleted = false;
        private Option<Result<T, Exception>> result = Option<Result<T, Exception>>.None;
        private ManualResetEvent executeCompleted = new(false);
        private Option<Func<T>> task;

        private List<Executable> continuations = new();

        public bool IsCompleted => this.isCompleted;

        public Task(MyThreadPool threadPool, Func<T> task)
        {
            this.task = task;
            this.threadPool = threadPool;
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

        public void Wait() => this.executeCompleted.WaitOne();

        public IMyTask<TNew> ContinueWith<TNew>(Func<T, TNew> continuation)
        {
            Interlocked.Increment(ref this.threadPool.tasksToEnqueue);
            if (!this.threadPool.keepExecuting)
            {
                Interlocked.Decrement(ref this.threadPool.tasksToEnqueue);
                this.threadPool.UpdateQueueIsEmpty();
                throw new InvalidOperationException("The thread pool is shut down.");
            }
            Task<TNew> newTask = new(this.threadPool, () => continuation(this.Result));
            lock (this.result)
            {
                if (Volatile.Read(ref this.isCompleted))
                {
                    this.threadPool.Submit(newTask);
                }
                else
                {
                    this.continuations.Add(newTask);
                }
            }

            return newTask;
        }
    }
}
