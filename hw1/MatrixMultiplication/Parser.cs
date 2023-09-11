namespace MatrixMultiplication;

/// <summary>Matrix parser.</summary>
public static class MatrixParser
{
    /// <summary>
    /// Parses matrix from given lines.
    /// Numbers in lines should be separated by one space character (' ').
    /// </summary>
    /// <param name="lines">lines with matrix elements.</param>
    /// <returns>Parsed matrix.</returns>
    public static int[,] ParseMatrix(string[] lines)
    {
        string[][] splitedLines = lines.Select(line => line.Split(' ')).ToArray();

        var matrix = new int[splitedLines.Length, splitedLines[0].Length];
        for (int i = 0; i < splitedLines.Length; i++)
        {
            if (splitedLines[i].Length != splitedLines[0].Length)
            {
                throw new ArgumentException("Given lines have different length");
            }

            for (int j = 0; j < splitedLines[i].Length; j++)
            {
                matrix[i, j] = int.Parse(splitedLines[i][j]);
            }
        }

        return matrix;
    }
}
