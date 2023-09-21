namespace Philosophers;

internal static class Program
{
    public static void Main(string[] args)
    {
        var table = new Table(2);

        Philosopher[] philosophers = table.Run();

        Console.ReadLine();

        table.Stop();
    }
}
