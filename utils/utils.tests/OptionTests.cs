namespace Utils.tests;

public class OptionTests
{
    [Test]
    public void IsSomeOnSomeReturnsTrue()
    {
        var value = new Option<int>.Some(42).IsSome();
        Assert.That(value, Is.True);
    }

    [Test]
    public void IsSomeOnNoneReturnsFalse()
    {
        var value = Option<int>.None.IsSome();
        Assert.That(value, Is.False);
    }

    [Test]
    public void MapOnSomeReturnsSome()
    {
        var value = new Option<int>.Some(42).Map(x => x + 1);
        Assert.That(value, Is.EqualTo(new Option<int>.Some(43)));
    }

    [Test]
    public void MapOnNoneReturnsNone()
    {
        var value = Option<int>.None.Map(x => x + 1);
        Assert.That(value, Is.EqualTo(Option<int>.None));
    }

    [Test]
    public void MapActionOnSomeCallsAction()
    {
        bool t = false;
        new Option<int>.Some(42).Map(x => t = true);
        Assert.That(t, Is.True);
    }

    [Test]
    public void MapActionOnNoneDoesNotCallAction()
    {
        bool t = false;
        Option<int>.None.Map(x => t = true);
        Assert.That(t, Is.False);
    }

    [Test]
    public void UnwrapOnSomeReturnsValue()
    {
        var value = new Option<int>.Some(42).Unwrap();
        Assert.That(value, Is.EqualTo(42));
    }

    [Test]
    public void UnwrapOnNoneThrowsException()
    {
        Assert.Throws<InvalidOperationException>(() => Option<int>.None.Unwrap());
    }

    [Test]
    public void ImplicitOperatorOnValueReturnsSome()
    {
        Option<int> value = 42;
        Assert.That(value, Is.EqualTo(new Option<int>.Some(42)));
    }
}
