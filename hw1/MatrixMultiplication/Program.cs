namespace MatrixMultiplication;

internal static class Program
{
    private const string HelpMessage =
        "Usage: MatrixMultiplication [-m <mode>] <first file> <second file> <output file>\n"
        + "  Correct modes: \"transposition\", \"multithreading\"\n"
        + "  When launched with nio mode, program will not use transposition or multithreading\n"
        + "  When launched with transposition mode, program will use transposition to speed up matrix multiplication\n"
        + "  When launched with multithreading mode, program will use both transposition and multithreading to speed up matrix multiplication\n"
        + "  Program reads two matricies from first and second files, multiplies them and writes result to output file.\n"
        + "  Numbers in files should be separated by one space character (' '), rows should be separated by newline character ('\\n').\n"
        + "  Example files:\n"
        + "  File 1:\n"
        + "    1 2 3\n"
        + "    4 5 6\n"
        + "  File 2:\n"
        + "    1 2\n"
        + "    3 4\n"
        + "    5 6\n"
        + "  Result:\n"
        + "    22 28\n"
        + "    49 64\n";

    private static void WriteToFile(int[,] matrix, string path)
    {
        File.WriteAllLines(
            path,
            from i in Enumerable.Range(0, matrix.GetLength(0))
            select string.Join(
                ' ',
                from j in Enumerable.Range(0, matrix.GetLength(1))
                select matrix[i, j]
            )
        );
    }

    private static int[,]? PerformMultiplication(
        Workspace.ModeType mode,
        int[,] matrix1,
        int[,] matrix2
    )
    {
        switch (mode)
        {
            case Workspace.ModeType.Default:
                return MartixMultiplier.Multiply(matrix1, matrix2);
            case Workspace.ModeType.Transposition:
                return MartixMultiplier.MultiplyWithTransposition(matrix1, matrix2);
            case Workspace.ModeType.Multithreading:
                return MartixMultiplier.MultiplyWithMultithreading(matrix1, matrix2);
            default:
                throw new ArgumentOutOfRangeException("Invalid mode");
        }
    }

    private static void Main(string[] args)
    {
        Workspace workspace;
        try
        {
            workspace = new Workspace(args);
        }
        catch (ArgumentException exception)
        {
            Console.WriteLine(exception.Message);
            return;
        }

        if (workspace.Mode == Workspace.ModeType.Help)
        {
            Console.WriteLine(HelpMessage);
            return;
        }

        int[,] matrix1;
        int[,] matrix2;
        try
        {
            matrix1 = Parser.ParseMatrix(File.ReadLines(workspace.InputFile1).ToArray());
            matrix2 = Parser.ParseMatrix(File.ReadLines(workspace.InputFile2).ToArray());
        }
        catch (IOException exception)
        {
            Console.WriteLine($"Cannot open file: {0}", exception.Message);
            return;
        }
        catch (ArgumentException exception)
        {
            Console.WriteLine($"Given file contains incorrect matrix: {0}", exception.Message);
            return;
        }

        int[,]? result = PerformMultiplication(workspace.Mode, matrix1, matrix2);

        if (result == null)
        {
            Console.WriteLine("Cannot multiply matrices - sizes don't match");
            return;
        }

        try
        {
            WriteToFile(result, workspace.OutputFile);
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Cannot write result to file: {0}", exception.Message);
            return;
        }
    }
}
