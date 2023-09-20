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
    public static IntMatrix? Multiply(IntMatrix a, IntMatrix b)
    {
        if (a.Width != b.Height)
        {
            return null;
        }

        var c = new IntMatrix(a.Height, b.Width);

        for (int i = 0; i < a.Height; i++)
        {
            for (int j = 0; j < b.Width; j++)
            {
                c[i, j] = 0;
                for (int k = 0; k < a.Width; k++)
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
    public static IntMatrix? MultiplyWithTransposition(IntMatrix a, IntMatrix b)
    {
        if (a.Width != b.Height)
        {
            return null;
        }

        var bTransposed = Transpose(b);
        var c = new IntMatrix(a.Height, b.Width);

        for (int i = 0; i < a.Height; i++)
        {
            for (int j = 0; j < b.Width; j++)
            {
                c[i, j] = 0;
                for (int k = 0; k < a.Width; k++)
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
    public static IntMatrix? MultiplyWithMultithreading(IntMatrix a, IntMatrix b)
    {
        if (a.Width != b.Height)
        {
            return null;
        }

        var bTransposed = TransposeMultithreaded(b);
        var c = new IntMatrix(a.Height, b.Width);

        var threads = Enumerable
            .Range(0, a.Height)
            .Select(
                i =>
                    new Thread(() =>
                    {
                        for (int j = 0; j < b.Width; j++)
                        {
                            c[i, j] = 0;
                            for (int k = 0; k < a.Width; k++)
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
    private static IntMatrix Transpose(IntMatrix a)
    {
        var transposed = new IntMatrix(a.Width, a.Height);
        for (int i = 0; i < a.Width; i++)
        {
            for (int j = 0; j < a.Height; j++)
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
    private static IntMatrix TransposeMultithreaded(IntMatrix a)
    {
        var transposed = new IntMatrix(a.Width, a.Height);
        var threads = Enumerable
            .Range(0, a.Width)
            .Select(
                i =>
                    new Thread(() =>
                    {
                        for (int j = 0; j < a.Height; j++)
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
