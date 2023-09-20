using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using MatrixMultiplication;

public class Benchmark
{
    [Params(1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048)]
    public int size { get; set; }
    private IntMatrix a = new IntMatrix(0, 1);
    private IntMatrix b = new IntMatrix(0, 0);

    [GlobalSetup]
    public void GlobalSetup()
    {
        a = new IntMatrix(size, size);
        b = new IntMatrix(size, size);
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
    public IntMatrix? Multiply() => MartixMultiplier.Multiply(a, b);

    [Benchmark]
    public IntMatrix? MultiplyWithTransposition() =>
        MartixMultiplier.MultiplyWithTransposition(a, b);

    [Benchmark]
    public IntMatrix? MultiplyWithThreads() => MartixMultiplier.MultiplyWithMultithreading(a, b);
}

internal class Program
{
    private static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<Benchmark>();
        Console.WriteLine(summary);
    }
}
