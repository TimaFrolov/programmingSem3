namespace MatrixMultiplication.tests;

public class ParserTests
{
    private static readonly (string[], int[,])[] TestCases =
    {
        (
            new string[] { "1 2 3", "4 5 6" },
            new int[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            }
        ),
        (
            new string[] { "1 2", "3 4", "5 6" },
            new int[,]
            {
                { 1, 2 },
                { 3, 4 },
                { 5, 6 }
            }
        ),
    };

    private static readonly string[][] TestCasesThrows =
    {
        new string[] { "1 2 3", "4 5" },
        new string[] { "1 2 3", "4 5 6", "7 8 9 10" },
    };

    [TestCaseSource(nameof(TestCases))]
    public void Test1((string[] lines, int[,] expected) testCase)
    {
        var actual = MatrixParser.ParseMatrix(testCase.lines);
        Assert.That(actual, Is.EqualTo(testCase.expected));
    }

    [TestCaseSource(nameof(TestCasesThrows))]
    public void Test2(string[] lines)
    {
        Assert.Throws<ArgumentException>(() => MatrixParser.ParseMatrix(lines));
    }
}