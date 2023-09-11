namespace MatrixMultiplication.tests;

public class MatrixMultiplierTests
{
    private static readonly (int[,], int[,], int[,]? TestCases)[] TestCases =
    {
        (
            new int[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            },
            new int[,]
            {
                { 1, 2 },
                { 3, 4 },
                { 5, 6 }
            },
            new int[,]
            {
                { 22, 28 },
                { 49, 64 }
            }
        ),
        (
            new int[,]
            {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 1 }
            },
            new int[,]
            {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 1 }
            },
            new int[,]
            {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 1 }
            }
        ),
        (
            new int[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            },
            new int[,]
            {
                { 1, 2 },
                { 3, 4 }
            },
            null
        )
    };

    public static Func<int[,], int[,], int[,]?>[] Functions =
    {
        MartixMultiplier.Multiply,
        MartixMultiplier.MultiplyWithTransposition,
        MartixMultiplier.MultiplyWithMultithreading,
    };

    [Test]
    public void Test(
        [ValueSource(nameof(TestCases))]
            (int[,] matrix1, int[,] matrix2, int[,]? expected) testCase,
        [ValueSource(nameof(Functions))] Func<int[,], int[,], int[,]?> function
    )
    {
        var actual = function(testCase.matrix1, testCase.matrix2);
        Assert.That(actual, Is.EqualTo(testCase.expected));
    }
}
