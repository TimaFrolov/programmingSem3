namespace MyThreadPool;

/// <summary>
/// Represents a thread that executes tasks.
/// </summary>
internal class MyThread
{
    private Thread underlying;
    private Func<Action> taskFactory;

    private volatile bool keepExecuting = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyThread"/> class with the specified task factory.
    /// </summary>
    /// <param name="taskFactory">The task factory.</param>
    public MyThread(Func<Action> taskFactory)
    {
        this.taskFactory = taskFactory;
        this.underlying = new Thread(this.EventLoop);
        this.underlying.Start();
    }

    /// <summary>
    /// Shuts down the thread.
    /// </summary>
    public void Shutdown() => this.keepExecuting = false;

    /// <summary>
    /// Blocks until the thread is finished.
    /// </summary>
    public void Wait() => this.underlying.Join();

    private void EventLoop()
    {
        while (this.keepExecuting)
        {
            this.taskFactory()();
        }
    }
}
