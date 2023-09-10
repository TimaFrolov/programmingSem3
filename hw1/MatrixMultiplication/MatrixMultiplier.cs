namespace MatrixMultiplication;

public static class MartixMultiplier
{
    public static int[,]? Multiply(int[,] a, int[,] b)
    {
        if (a.GetLength(1) != b.GetLength(0))
        {
            return null;
        }

        int[,] c = new int[a.GetLength(0), b.GetLength(1)];

        for (int i = 0; i < a.GetLength(0); i++)
        {
            for (int j = 0; j < b.GetLength(1); j++)
            {
                c[i, j] = 0;
                for (int k = 0; k < a.GetLength(1); k++)
                {
                    c[i, j] += a[i, k] * b[k, j];
                }
            }
        }

        return c;
    }

    public static int[,]? MultiplyWithTransposition(int[,] a, int[,] b)
    {
        if (a.GetLength(1) != b.GetLength(0))
        {
            return null;
        }

        var bTransposed = Transpose(b);
        int[,] c = new int[a.GetLength(0), b.GetLength(1)];

        for (int i = 0; i < a.GetLength(0); i++)
        {
            for (int j = 0; j < b.GetLength(1); j++)
            {
                c[i, j] = 0;
                for (int k = 0; k < a.GetLength(1); k++)
                {
                    c[i, j] += a[i, k] * bTransposed[j, k];
                }
            }
        }

        return c;
    }

    public static int[,]? MultiplyWithMultithreading(int[,] a, int[,] b)
    {
        if (a.GetLength(1) != b.GetLength(0))
        {
            return null;
        }

        var bTransposed = TransposeMultithreaded(b);
        int[,] c = new int[a.GetLength(0), b.GetLength(1)];

        var threads = Enumerable
            .Range(0, a.GetLength(0))
            .Select(
                i =>
                    new Thread(() =>
                    {
                        for (int j = 0; j < b.GetLength(1); j++)
                        {
                            c[i, j] = 0;
                            for (int k = 0; k < a.GetLength(1); k++)
                            {
                                c[i, j] += a[i, k] * bTransposed[j, k];
                            }
                        }
                    })
            )
            .ToArray();

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        return c;
    }

    private static int[,] Transpose(int[,] a)
    {
        var transposed = new int[a.GetLength(1), a.GetLength(0)];
        for (int i = 0; i < a.GetLength(1); i++)
        {
            for (int j = 0; j < a.GetLength(0); j++)
            {
                transposed[i, j] = a[j, i];
            }
        }

        return transposed;
    }

    private static int[,] TransposeMultithreaded(int[,] a)
    {
        var transposed = new int[a.GetLength(1), a.GetLength(0)];
        var threads = Enumerable
            .Range(0, a.GetLength(1))
            .Select(
                i =>
                    new Thread(() =>
                    {
                        for (int j = 0; j < a.GetLength(0); j++)
                        {
                            transposed[i, j] = a[j, i];
                        }
                    })
            )
            .ToArray();

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        return transposed;
    }
}
