namespace MatrixMultiplication.tests;

public class ParserTests
{
    private static readonly (string[], IntMatrix)[] TestCases =
    {
        (
            new string[] { "1 2 3", "4 5 6" },
            new IntMatrix(
                new int[,]
                {
                    { 1, 2, 3 },
                    { 4, 5, 6 }
                }
            )
        ),
        (
            new string[] { "1 2", "3 4", "5 6" },
            new IntMatrix(
                new int[,]
                {
                    { 1, 2 },
                    { 3, 4 },
                    { 5, 6 }
                }
            )
        ),
    };

    private static readonly string[][] TestCasesThrows =
    {
        new string[] { "1 2 3", "4 5" },
        new string[] { "1 2 3", "4 5 6", "7 8 9 10" },
    };

    [TestCaseSource(nameof(TestCases))]
    public void CorrectParseTest((string[] lines, IntMatrix expected) testCase)
    {
        var actual = MatrixParser.ParseMatrix(testCase.lines);
        Assert.That(actual, Is.EqualTo(testCase.expected));
    }

    [TestCaseSource(nameof(TestCasesThrows))]
    public void ParseErrorTest(string[] lines)
    {
        Assert.Throws<ArgumentException>(() => MatrixParser.ParseMatrix(lines));
    }
}
