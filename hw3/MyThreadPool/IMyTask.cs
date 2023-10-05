namespace MyThreadPool;

/// <summary>
/// Represents a task that is executed by a thread pool.
/// </summary>
/// <typeparam name="T">The type of the task result.</typeparam>
public interface IMyTask<T>
{
    /// <summary>
    /// Gets a value indicating whether the task has completed.
    /// </summary>
    public bool IsCompleted { get; }

    /// <summary>
    /// Gets the result of the task. Blocks until the task is completed.
    /// </summary>
    /// <throws><see cref="AggregateException"/> if provided function threw an exception.</throws>
    public T Result { get; }

    /// <summary>
    /// Blocks until the task is completed.
    /// </summary>
    public void Wait();

    /// <summary>
    /// Continues the task with a new task. The new task is executed when the current task is completed.
    /// The result of the current task is passed to the provided continuation function.
    /// </summary>
    /// <typeparam name="TNew">The type of the new task result.</typeparam>
    /// <param name="continuation">The continuation function.</param>
    /// <returns>A task that represents the continuation.</returns>
    /// <throws><see cref="InvalidOperationException"/> if thread pool executing this task is shut down.</throws>
    public IMyTask<TNew> ContinueWith<TNew>(Func<T, TNew> continuation);

    /// <summary>
    /// Continues the task with a new task. The new task is executed when the current task is completed.
    /// The result of the current task is passed to the provided continuation function.
    /// </summary>
    /// <param name="continuation">The continuation function.</param>
    /// <returns>A task that represents the continuation.</returns>
    /// <throws><see cref="InvalidOperationException"/> if thread pool executing this task is shut down.</throws>
    public IMyTask ContinueWith(Action<T> continuation);
}

/// <summary>
/// Represents a task that is executed by a thread pool and returns no result.
/// </summary>
public interface IMyTask : IMyTask<Monostate>
{
    /// <summary>
    /// Continues the task with a new task. The new task is executed when the current task is completed.
    /// </summary>
    /// <typeparam name="TNew">The type of the new task result.</typeparam>
    /// <param name="continuation">The continuation function.</param>
    /// <returns>A task that represents the continuation.</returns>
    /// <throws><see cref="InvalidOperationException"/> if thread pool executing this task is shut down.</throws>
    public IMyTask<TNew> ContinueWith<TNew>(Func<TNew> continuation);

    /// <summary>
    /// Continues the task with a new task. The new task is executed when the current task is completed.
    /// </summary>
    /// <param name="continuation">The continuation function.</param>
    /// <returns>A task that represents the continuation.</returns>
    /// <throws><see cref="InvalidOperationException"/> if thread pool executing this task is shut down.</throws>
    public IMyTask ContinueWith(Action continuation);
}
