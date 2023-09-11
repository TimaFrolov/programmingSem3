namespace MatrixMultiplication;

/// <summary>Collection of functions that perform matrix multiplication.</summary>
public static class MartixMultiplier
{
    /// <summary>
    /// Multiplies two matrices.
    /// </summary>
    /// <param name="a">First matrix (with sizes N x K).</param>
    /// <param name="b">Second matrix (with sizes K x M).</param>
    /// <returns>Result of multiplication (with sizes N x M).</returns>
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

    /// <summary>
    /// Multiplies two matrices. Uses transposition of the second matrix to improve performance.
    /// </summary>
    /// <param name="a">First matrix (with sizes N x K).</param>
    /// <param name="b">Second matrix (with sizes K x M).</param>
    /// <returns>Result of multiplication (with sizes N x M).</returns>
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

    /// <summary>
    /// Multiplies two matrices.
    /// Uses transposition of the second matrix to improve performance.
    /// Uses multithreading to improve performance.
    /// </summary>
    /// <param name="a">First matrix (with sizes N x K).</param>
    /// <param name="b">Second matrix (with sizes K x M).</param>
    /// <returns>Result of multiplication (with sizes N x M).</returns>
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

    /// <summary>
    /// Transposes matrix.
    /// </summary>
    /// <param name="a">Matrix to get transposed version of (with sizes N x M).</param>
    /// <returns>Result of transposition (with sizes M x N).</returns>
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

    /// <summary>
    /// Transposes matrix.
    /// Uses multithreading to improve performance.
    /// </summary>
    /// <param name="a">Matrix to get transposed version of (with sizes N x M).</param>
    /// <returns>Result of transposition (with sizes M x N).</returns>
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
