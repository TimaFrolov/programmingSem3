namespace Philosophers;

public class Table
{
    private Philosopher[] philosophers;
    private Thread[] threads;
    private Mutex[] forks;
    private bool running = false;

    public Table(int philosophersAmount)
    {
        if (philosophersAmount < 2)
        {
            throw new ArgumentException("There should be at least two philosophers");
        }

        this.forks = Enumerable.Range(0, philosophersAmount).Select(_ => new Mutex()).ToArray();
        this.philosophers = Enumerable
            .Range(0, philosophersAmount)
            .Select(i => new Philosopher(forks[i], forks[(i + 1) % philosophersAmount], i != 0))
            .ToArray();
        this.threads = philosophers
            .Select(philosopher => new Thread(philosopher.LifeCycle))
            .ToArray();
    }

    public Philosopher[] Run()
    {
        if (this.running)
        {
            throw new InvalidOperationException("Table is already running");
        }

        this.running = true;
        foreach (var thread in threads)
        {
            thread.Start();
        }

        return this.philosophers;
    }

    public void Stop()
    {
        foreach (var philosopher in this.philosophers)
        {
            philosopher.LeaveTable();
        }

        foreach (var thread in this.threads) {
            thread.Join();
        }
    }
}
