namespace MatrixMultiplication;

/// <summary> Two dimensional matrix with integer elements. </summary>
public class IntMatrix
{
    private int[,] underlying;

    /// <summary> Gets height of the matrix. </summary>
    public int Height => underlying.GetLength(0);

    /// <summary> Gets width of the matrix. </summary>
    public int Width => underlying.GetLength(1);

    /// <summary>
    /// Initializes a new instance of the <see cref="IntMatrix"/> class from given matrix.
    /// </summary>
    /// <param name="matrix">Underlying matrix.</param>
    public IntMatrix(int[,] matrix) => this.underlying = matrix;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntMatrix"/> class with given dimensions.
    /// </summary>
    /// <param name="height">Height of the matrix.</param>
    /// <param name="width">Width of the matrix.</param>
    public IntMatrix(int height, int width) => this.underlying = new int[height, width];

    /// <summary> Gets or sets element of the matrix with given position.</summary>
    /// <param name="row">Row index of the element.</param>
    /// <param name="column">Column index of the element.</param>
    public int this[int row, int column]
    {
        get => this.underlying[row, column];
        set => this.underlying[row, column] = value;
    }

    /// <summary>
    /// Compares two matrices.
    /// </summary>
    /// <param name="other">Matrix to compare with.</param>
    /// <returns>True if matrices are equal, false otherwise.</returns>
    public override bool Equals(object? other)
    {
        if (!(other is IntMatrix matrix))
        {
            return false;
        }
        if (this.Height != matrix.Height || this.Width != matrix.Width)
        {
            return false;
        }

        for (int i = 0; i < this.Height; i++)
        {
            for (int j = 0; j < this.Width; j++)
            {
                if (this[i, j] != matrix[i, j])
                {
                    return false;
                }
            }
        }

        return true;
    }
}
