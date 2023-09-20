namespace MatrixMultiplication;

/// <summary>Collection of functions that perform matrix multiplication.</summary>
public static class MartixMultiplier
{
    /// <summary>
    /// Multiplies two matrices.
    /// </summary>
    /// <param name="firstMatrix">First matrix (with sizes N x K).</param>
    /// <param name="secondMatrix">Second matrix (with sizes K x M).</param>
    /// <returns>Result of multiplication (with sizes N x M).</returns>
    public static IntMatrix? Multiply(IntMatrix firstMatrix, IntMatrix secondMatrix)
    {
        if (firstMatrix.Width != secondMatrix.Height)
        {
            return null;
        }

        var answer = new IntMatrix(firstMatrix.Height, secondMatrix.Width);

        for (int i = 0; i < firstMatrix.Height; i++)
        {
            for (int j = 0; j < secondMatrix.Width; j++)
            {
                answer[i, j] = Enumerable
                    .Range(0, firstMatrix.Width)
                    .Sum(k => firstMatrix[i, k] * secondMatrix[k, j]);
            }
        }

        return answer;
    }

    /// <summary>
    /// Multiplies two matrices. Uses transposition of the second matrix to improve performance.
    /// </summary>
    /// <param name="firstMatrix">First matrix (with sizes N x K).</param>
    /// <param name="secondMatrix">Second matrix (with sizes K x M).</param>
    /// <returns>Result of multiplication (with sizes N x M).</returns>
    public static IntMatrix? MultiplyWithTransposition(
        IntMatrix firstMatrix,
        IntMatrix secondMatrix
    )
    {
        if (firstMatrix.Width != secondMatrix.Height)
        {
            return null;
        }

        var secondMatrixTransposed = Transpose(secondMatrix);
        var answer = new IntMatrix(firstMatrix.Height, secondMatrix.Width);

        for (int i = 0; i < firstMatrix.Height; i++)
        {
            for (int j = 0; j < secondMatrix.Width; j++)
            {
                answer[i, j] = Enumerable
                    .Range(0, firstMatrix.Width)
                    .Sum(k => firstMatrix[i, k] * secondMatrixTransposed[j, k]);
            }
        }

        return answer;
    }

    /// <summary>
    /// Multiplies two matrices.
    /// Uses transposition of the second matrix to improve performance.
    /// Uses multithreading to improve performance.
    /// </summary>
    /// <param name="firstMatrix">First matrix (with sizes N x K).</param>
    /// <param name="secondMatrix">Second matrix (with sizes K x M).</param>
    /// <returns>Result of multiplication (with sizes N x M).</returns>
    public static IntMatrix? MultiplyWithMultithreading(
        IntMatrix firstMatrix,
        IntMatrix secondMatrix
    )
    {
        if (firstMatrix.Width != secondMatrix.Height)
        {
            return null;
        }

        var secondMatrixTransposed = TransposeMultithreaded(secondMatrix);
        var answer = new IntMatrix(firstMatrix.Height, secondMatrix.Width);

        var threads = Enumerable
            .Range(0, firstMatrix.Height)
            .Select(
                i =>
                    new Thread(() =>
                    {
                        for (int j = 0; j < secondMatrix.Width; j++)
                        {
                            answer[i, j] = Enumerable
                                .Range(0, firstMatrix.Width)
                                .Sum(k => firstMatrix[i, k] * secondMatrixTransposed[j, k]);
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

        return answer;
    }

    /// <summary>
    /// Transposes matrix.
    /// </summary>
    /// <param name="matrix">Matrix to get transposed version of (with sizes N x M).</param>
    /// <returns>Result of transposition (with sizes M x N).</returns>
    private static IntMatrix Transpose(IntMatrix matrix)
    {
        var transposed = new IntMatrix(matrix.Width, matrix.Height);
        for (int i = 0; i < matrix.Width; i++)
        {
            for (int j = 0; j < matrix.Height; j++)
            {
                transposed[i, j] = matrix[j, i];
            }
        }

        return transposed;
    }

    /// <summary>
    /// Transposes matrix.
    /// Uses multithreading to improve performance.
    /// </summary>
    /// <param name="matrix">Matrix to get transposed version of (with sizes N x M).</param>
    /// <returns>Result of transposition (with sizes M x N).</returns>
    private static IntMatrix TransposeMultithreaded(IntMatrix matrix)
    {
        var transposed = new IntMatrix(matrix.Width, matrix.Height);
        var threads = Enumerable
            .Range(0, matrix.Width)
            .Select(
                i =>
                    new Thread(() =>
                    {
                        for (int j = 0; j < matrix.Height; j++)
                        {
                            transposed[i, j] = matrix[j, i];
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
