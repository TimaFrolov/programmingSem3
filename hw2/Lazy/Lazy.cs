namespace Lazy;

using Utils;

/// <summary>
/// Interface for lazy evaluation.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public interface ILazy<T>
{
    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <returns>The value.</returns>
    /// <exception>If the factory function throws an exception.</exception>
    T Get();
}

/// <summary>
/// A lazy evaluation implementation.
/// </summary>
/// <remarks>
/// This implementation is not thread-safe.
/// </remarks>
/// <typeparam name="T">The type of the value.</typeparam>
public sealed class Lazy<T> : ILazy<T>
{
    private Option<Func<T>> factory;
    private Option<Result<T, Exception>> value = Option<Result<T, Exception>>.None;

    /// <summary>
    /// Initializes a new instance of the <see cref="Lazy{T}"/> class.
    /// </summary>
    /// <param name="factory">The factory function.</param>
    /// <returns>The lazy evaluation.</returns>
    public Lazy(Func<T> factory) => this.factory = factory;

    /// <inheritdoc cref="ILazy{T}"/>
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

/// <summary>
/// A thread-safe lazy evaluation implementation.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public sealed class ParallelLazy<T> : ILazy<T>
{
    private Option<Func<T>> factory;
    private Option<Result<T, Exception>> value = Option<Result<T, Exception>>.None;

    private int valueStartedProducing = 0;
    private volatile bool isValueProduced = false;
    private ManualResetEvent valueProducedEvent = new ManualResetEvent(false);

    /// <summary>
    /// Initializes a new instance of the <see cref="ParallelLazy{T}"/> class.
    /// </summary>
    /// <param name="factory">The factory function.</param>
    /// <returns>The lazy evaluation.</returns>
    public ParallelLazy(Func<T> factory) => this.factory = factory;

    /// <inheritdoc cref="ILazy{T}"/>
    public T Get()
    {
        if (Interlocked.Increment(ref this.valueStartedProducing) == 1)
        {
            Result<T, Exception> value = Try<Exception>.Call(this.factory.Unwrap());

            this.value = value;
            this.isValueProduced = true;
            this.valueProducedEvent.Set();

            this.factory = Option<Func<T>>.None;
            return value.UnwrapOrElse(err => throw err);
        }

        Interlocked.Exchange(ref this.valueStartedProducing, 1);

        if (!this.isValueProduced)
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
