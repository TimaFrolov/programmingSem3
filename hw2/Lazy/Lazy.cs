namespace Lazy;
using Utils;

public interface ILazy<T>
{
    T Get();
}

public sealed class Lazy<T> : ILazy<T>
{
    private Option<Func<T>> factory;
    private Option<Result<T, Exception>> value = Option<Result<T, Exception>>.None;

    public Lazy(Func<T> factory) => this.factory = factory;

    public T Get()
    {
        this.factory.Map(func =>
        {
            this.value = Try<Exception>.Call(this.factory.Unwrap());
            this.factory = Option<Func<T>>.None;
        });

        return this.value.Unwrap().UnwrapOrElse(err => throw err);
    }
}

public sealed class AtomicLazy<T> : ILazy<T>
{
    private Option<Func<T>> factory;
    private Option<Result<T, Exception>> value = Option<Result<T, Exception>>.None;

    private int valueStartedProducing = 0;
    private bool valueProduced = false;
    private ManualResetEvent valueProducedEvent = new ManualResetEvent(false);

    public AtomicLazy(Func<T> factory) => this.factory = factory;

    public T Get()
    {
        if (Interlocked.Increment(ref this.valueStartedProducing) == 1)
        {
            Result<T, Exception> value = Try<Exception>.Call(this.factory.Unwrap());

            this.value = value;
            Volatile.Write(ref this.valueProduced, true);
            this.valueProducedEvent.Set();

            this.factory = Option<Func<T>>.None;
            return value.UnwrapOrElse(err => throw err);
        }

        Interlocked.Exchange(ref this.valueStartedProducing, 1);

        if (!this.valueProduced)
        {
            this.valueProducedEvent.WaitOne();
            return Volatile.Read(ref this.value).Unwrap().UnwrapOrElse(err => throw err);
        }
        else
        {
            return this.value.Unwrap().UnwrapOrElse(err => throw err);
        }
    }
}
