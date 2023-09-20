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

    public int[,] GetUnderlying() => underlying;
}
