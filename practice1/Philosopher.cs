namespace Philosophers;

public class Philosopher
{
    private Mutex leftFork;
    private Mutex rightFork;
    public int timesEaten {get; private set;} = 0;
    private Random rng = new Random();
    private bool leftHanded;
    private volatile bool sittingAtTable = true;

    public Philosopher(Mutex leftFork, Mutex rightFork, bool leftHanded)
    {
        this.leftFork = leftFork;
        this.rightFork = rightFork;
        this.leftHanded = leftHanded;
    }

    public void LifeCycle()
    {
        Console.Write(
            "Philosopher {0} is joining the table\n",
            Thread.CurrentThread.ManagedThreadId
        );
        this.sittingAtTable = true;

        while (this.sittingAtTable)
        {
            // Getting forks
            Console.Write(
                "Philosopher {0} trying to aquire {1} fork\n",
                Thread.CurrentThread.ManagedThreadId,
                this.leftHanded ? "left" : "right"
            );
            if (this.leftHanded)
            {
                leftFork.WaitOne();
            }
            else
            {
                rightFork.WaitOne();
            }
            Console.Write(
                "Philosopher {0} aquired {1} fork. Trying to aquire {2} fork\n",
                Thread.CurrentThread.ManagedThreadId,
                this.leftHanded ? "left" : "right",
                this.leftHanded ? "right" : "left"
            );
            if (this.leftHanded)
            {
                rightFork.WaitOne();
            }
            else
            {
                leftFork.WaitOne();
            }

            // Eating
            Console.Write("Philosopher {0} is eating\n", Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(1);
            timesEaten++;

            // Releasing forks
            rightFork.ReleaseMutex();
            leftFork.ReleaseMutex();

            // Thinking
            Console.Write("Philosopher {0} is thinking\n", Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(1);
        }

        Console.Write(
            "Philosopher {0} is leaving the table\n",
            Thread.CurrentThread.ManagedThreadId
        );
    }

    public void LeaveTable() => this.sittingAtTable = false;
}
