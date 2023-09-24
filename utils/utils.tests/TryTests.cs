namespace Utils.tests;

public class TryTests
{
    [Test]
    public void CallOnFunctionThatDoesNotThrowReturnsOk()
    {
        var value = Try<Exception>.Call(() => 42);
        Assert.That(value, Is.EqualTo(new Result<int, Exception>.Ok(42)));
    }

    [Test]
    public void CallOnFunctionThatThrowsReturnsErr()
    {
        var value = Try<Exception>.Call<int>(() => throw new Exception());
        Assert.That(value, Is.InstanceOf<Result<int, Exception>.Err>());
    }

    [Test]
    public void CallOnFunctionThatThrowsReturnsErrWithIncorrectException()
    {
        Assert.Throws<Exception>(
            () => Try<InvalidOperationException>.Call<int>(() => throw new Exception())
        );
    }
}
