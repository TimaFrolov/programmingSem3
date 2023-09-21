namespace MatrixMultiplication.tests;

public class MatrixMultiplierTests
{
    private static readonly (IntMatrix, IntMatrix, IntMatrix? TestCases)[] TestCases =
    {
        (
            new IntMatrix(
                new int[,]
                {
                    { 1, 2, 3 },
                    { 4, 5, 6 }
                }
            ),
            new IntMatrix(
                new int[,]
                {
                    { 1, 2 },
                    { 3, 4 },
                    { 5, 6 }
                }
            ),
            new IntMatrix(
                new int[,]
                {
                    { 22, 28 },
                    { 49, 64 }
                }
            )
        ),
        (
            new IntMatrix(
                new int[,]
                {
                    { 1, 0, 0 },
                    { 0, 1, 0 },
                    { 0, 0, 1 }
                }
            ),
            new IntMatrix(
                new int[,]
                {
                    { 1, 0, 0 },
                    { 0, 1, 0 },
                    { 0, 0, 1 }
                }
            ),
            new IntMatrix(
                new int[,]
                {
                    { 1, 0, 0 },
                    { 0, 1, 0 },
                    { 0, 0, 1 }
                }
            )
        ),
        (
            new IntMatrix(
                new int[,]
                {
                    { 1, 2, 3 },
                    { 4, 5, 6 }
                }
            ),
            new IntMatrix(
                new int[,]
                {
                    { 1, 2 },
                    { 3, 4 }
                }
            ),
            null
        )
    };

    public static Func<IntMatrix, IntMatrix, IntMatrix?>[] Functions =
    {
        MartixMultiplier.Multiply,
        MartixMultiplier.MultiplyWithTransposition,
        MartixMultiplier.MultiplyWithMultithreading,
    };

    [Test]
    public void MultiplyMatrixTest(
        [ValueSource(nameof(TestCases))]
            (IntMatrix matrix1, IntMatrix matrix2, IntMatrix? expected) testCase,
        [ValueSource(nameof(Functions))] Func<IntMatrix, IntMatrix, IntMatrix?> function
    )
    {
        var actual = function(testCase.matrix1, testCase.matrix2);
        Assert.That(actual, Is.EqualTo(testCase.expected));
    }
}
