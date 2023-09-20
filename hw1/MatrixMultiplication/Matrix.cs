namespace MatrixMultiplication;

public class IntMatrix
{
    private int[,] underlying;

    public int Height => underlying.GetLength(0);
    public int Width => underlying.GetLength(1);

    public IntMatrix(int[,] matrix) => this.underlying = matrix;

    public IntMatrix(int height, int width) => this.underlying = new int[height, width];

    public int this[int i, int j]
    {
        get => this.underlying[i, j];
        set => this.underlying[i, j] = value;
    }

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
