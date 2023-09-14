using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

public class Benchmark
{
    [Params(1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048)]
    public int size { get; set; }
    private int[,] a = new int[0, 1];
    private int[,] b = new int[0, 0];

    [GlobalSetup]
    public void GlobalSetup()
    {
        a = new int[size, size];
        b = new int[size, size];
        var random = new Random();
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                a[i, j] = random.Next();
                b[i, j] = random.Next();
            }
        }
    }

    [Benchmark]
    public void Multiply()
    {
        MatrixMultiplication.MartixMultiplier.Multiply(a, b);
    }

    [Benchmark]
    public void MultiplyWithTransposition()
    {
        MatrixMultiplication.MartixMultiplier.MultiplyWithTransposition(a, b);
    }

    [Benchmark]
    public void MultiplyWithThreads()
    {
        MatrixMultiplication.MartixMultiplier.MultiplyWithMultithreading(a, b);
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<Benchmark>();
        Console.WriteLine(summary);
    }
}
