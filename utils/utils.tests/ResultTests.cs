namespace Utils.tests;

public class ResultTests
{
    [Test]
    public void UnwrapOrElseOnOkReturnsValue()
    {
        var value = new Result<int, int>.Ok(42).UnwrapOrElse(_ => 0);
        Assert.That(value, Is.EqualTo(42));
    }

    [Test]
    public void UnwrapOrElseOnErrCallsFunction()
    {
        var value = new Result<int, int>.Err(42).UnwrapOrElse(_ => 0);
        Assert.That(value, Is.EqualTo(0));
    }

    [Test]
    public void IsOkOnOkReturnsTrue()
    {
        var value = new Result<int, int>.Ok(42).IsOk();
        Assert.That(value, Is.True);
    }

    [Test]
    public void IsOkOnErrReturnsFalse()
    {
        var value = new Result<int, int>.Err(42).IsOk();
        Assert.That(value, Is.False);
    }

    [Test]
    public void IsErrOnOkReturnsFalse()
    {
        var value = new Result<int, int>.Ok(42).IsErr();
        Assert.That(value, Is.False);
    }

    [Test]
    public void IsErrOnErrReturnsTrue()
    {
        var value = new Result<int, int>.Err(42).IsErr();
        Assert.That(value, Is.True);
    }

    [Test]
    public void UnwrapErrOnOkThrowsException()
    {
        Assert.Throws<InvalidOperationException>(() => new Result<int, int>.Ok(42).UnwrapErr());
    }

    [Test]
    public void UnwrapErrOnErrReturnsValue()
    {
        var value = new Result<int, int>.Err(42).UnwrapErr();
        Assert.That(value, Is.EqualTo(42));
    }
}
