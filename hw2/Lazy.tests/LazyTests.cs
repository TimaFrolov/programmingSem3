namespace Lazy.tests;

internal class Counter<T>
{
    private volatile int callsCount;
    public int CallsCount
    {
        get => this.callsCount;
        private set => this.callsCount = value;
    }
    private Func<T> func;

    public Counter(Func<T> func)
    {
        this.CallsCount = 0;
        this.func = func;
    }

    public T Call()
    {
        this.CallsCount++;
        return func();
    }
}

internal class TestException : Exception
{
    public TestException()
        : base() { }
}

public class LazyTests
{
    private static readonly Func<Func<int>, ILazy<int>>[] intConstructors =
    {
        func => new Lazy<int>(func),
        func => new LazyLock<int>(func),
    };

    [Test]
    public void TestGet(
        [ValueSource(nameof(intConstructors))] Func<Func<int>, ILazy<int>> lazyConstructor
    )
    {
        var counter = new Counter<int>(() => 42);
        var lazy = lazyConstructor(counter.Call);

        Assert.That(counter.CallsCount, Is.EqualTo(0));

        var first = lazy.Get();
        Assert.That(counter.CallsCount, Is.EqualTo(1));
        Assert.That(first, Is.EqualTo(42));

        var second = lazy.Get();
        Assert.That(counter.CallsCount, Is.EqualTo(1));
        Assert.That(second, Is.EqualTo(42));

        var third = lazy.Get();
        Assert.That(counter.CallsCount, Is.EqualTo(1));
        Assert.That(third, Is.EqualTo(42));
    }

    [Test]
    public void TestThrows(
        [ValueSource(nameof(intConstructors))] Func<Func<int>, ILazy<int>> lazyConstructor
    )
    {
        var exception = new TestException();
        var counter = new Counter<int>(() => throw exception);
        var lazy = lazyConstructor(counter.Call);

        Assert.That(counter.CallsCount, Is.EqualTo(0));

        var first = Utils.Try<TestException>.Call(lazy.Get);
        Assert.That(counter.CallsCount, Is.EqualTo(1));
        Assert.That(first.IsErr(), Is.True);
        Assert.That(first.UnwrapErr(), Is.EqualTo(exception));

        var second = Utils.Try<TestException>.Call(lazy.Get);
        Assert.That(counter.CallsCount, Is.EqualTo(1));
        Assert.That(second.IsErr(), Is.True);
        Assert.That(second.UnwrapErr(), Is.EqualTo(exception));

        var third = Utils.Try<TestException>.Call(lazy.Get);
        Assert.That(counter.CallsCount, Is.EqualTo(1));
        Assert.That(third.IsErr(), Is.True);
        Assert.That(third.UnwrapErr(), Is.EqualTo(exception));
    }

    [Test]
    public void TestLazyLock()
    {
        var counter = new Counter<int>(() => 42);
        var lazy = new LazyLock<int>(counter.Call);
        var @event = new ManualResetEvent(false);

        var threads = Enumerable
            .Range(0, 16)
            .Select(
                _ =>
                    new Thread(() =>
                    {
                        @event.WaitOne();
                        var result = lazy.Get();
                        Assert.That(counter.CallsCount, Is.EqualTo(1));
                        Assert.That(result, Is.EqualTo(42));
                    })
            )
            .ToList();

        foreach (var thread in threads)
        {
            thread.Start();
        }
        @event.Set();

        foreach (var thread in threads)
        {
            thread.Join();
        }
    }
}
