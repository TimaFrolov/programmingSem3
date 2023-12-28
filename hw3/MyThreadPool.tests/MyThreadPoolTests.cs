namespace MyThreadPool.tests;

public class MyThreadPoolTests
{
    [Test]
    public void CreateTaskRunsTask()
    {
        int x = 0;
        MyThreadPool pool = new(2);
        var task = pool.Submit(() => Volatile.Write(ref x, 1));
        task.Wait();
        Assert.That(Volatile.Read(ref x), Is.EqualTo(1));
    }

    [Test]
    public void TaskActuallyReturnsResult()
    {
        MyThreadPool pool = new(2);
        var task = pool.Submit(() => 1);
        Assert.That(task.Result, Is.EqualTo(1));
    }

    [Test]
    public void ContinueWithPassesTaskResult()
    {
        MyThreadPool pool = new(2);
        var task = pool.Submit(() => 1).ContinueWith(x => x + 1);
        Assert.That(task.Result, Is.EqualTo(2));
    }

    [Test]
    public void ContinueWithOnSameTaskWorks()
    {
        MyThreadPool pool = new(2);
        var task = pool.Submit(() => 1);
        var task2 = task.ContinueWith(x => x + 7);
        var task3 = task.ContinueWith(x => x + 13);
        Assert.That(task2.Result, Is.EqualTo(8));
        Assert.That(task3.Result, Is.EqualTo(14));
    }

    [Test]
    public void IsCompletedWorks()
    {
        MyThreadPool pool = new(2);
        ManualResetEvent mre = new(false);
        var task = pool.Submit(() => mre.WaitOne());
        Assert.That(task.IsCompleted, Is.False);
        mre.Set();
        task.Wait();
        Assert.That(task.IsCompleted, Is.True);
    }

    [Test]
    public void PoolActuallyCreatesNThreads()
    {
        var initialProcessesAmount = System.Diagnostics.Process.GetCurrentProcess().Threads.Count;
        MyThreadPool pool = new(5);
        Assert.That(
            System.Diagnostics.Process.GetCurrentProcess().Threads.Count,
            Is.EqualTo(initialProcessesAmount + 5)
        );
    }

    [Test]
    public void ShutodwnDestroysThreads()
    {
        var initialProcessesAmount = System.Diagnostics.Process.GetCurrentProcess().Threads.Count;
        MyThreadPool pool = new(5);
        pool.Shutdown();
        Assert.That(
            System.Diagnostics.Process.GetCurrentProcess().Threads.Count,
            Is.EqualTo(initialProcessesAmount)
        );
    }

    [Test]
    public void TaskThrowsAggregateExceptionOnExceptionInProvidedFunction()
    {
        MyThreadPool pool = new(2);
        var task = pool.Submit(() => throw new ArgumentOutOfRangeException());
        Assert.Throws<AggregateException>(() => _ = task.Result);
    }

    [Test]
    public void ParametrizedTaskThrowsAggregateExceptionOnExceptionInProvidedFunction()
    {
        MyThreadPool pool = new(2);
        var task = pool.Submit<int>(() => throw new ArgumentOutOfRangeException());
        Assert.Throws<AggregateException>(() => _ = task.Result);
    }
}
