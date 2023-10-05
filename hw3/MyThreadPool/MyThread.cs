using System.Collections.Concurrent;

internal class MyThread
{
    public bool IsBusy { get; private set; } = false;

    private Thread underlying;
    private Func<Action> taskFactory;

    private volatile bool keepExecuting = true;

    public MyThread(Func<Action> taskFactory)
    {
        this.taskFactory = taskFactory;
        this.underlying = new Thread(this.EventLoop);
        this.underlying.Start();
    }

    private void EventLoop()
    {
        while (this.keepExecuting)
        {
            Action task = this.taskFactory();
            this.IsBusy = true;
            task();
            this.IsBusy = false;
        }
    }

    public void Shutdown() => this.keepExecuting = false;

    public void Wait() => this.underlying.Join();
}
