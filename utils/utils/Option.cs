namespace Utils;

/// <summary>
/// Represents an optional value.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public record Option<T>
{
    /// <summary>
    /// Represents a value.
    /// </summary>
    /// <param name="value">The value.</param>
    public sealed record Some(T value) : Option<T>;

    /// <summary>
    /// Represents no value.
    /// </summary>
    /// <remarks>
    /// This is a singleton class. Use <see cref="Option{T}.None"/> to access the instance.
    /// </remarks>
    public sealed record _None : Option<T>;

    /// <summary>
    /// Represents no value.
    /// </summary>
    /// <remarks>
    /// This is a singleton instance of <see cref="Option{T}._None"/>.
    /// </remarks>
    public static readonly _None None = new _None();

    /// <summary>
    /// Implicitly converts a value to an option.
    /// </summary>
    /// <param name="value">The value.</param>
    public static implicit operator Option<T>(T value) => new Some(value);

    /// <summary>
    /// Checks if the option is a value.
    /// </summary>
    /// <returns><c>true</c> if the option is a value, <c>false</c> otherwise.</returns>
    public bool IsSome() => this is Some;

    /// <summary>
    /// Maps the option to a new option.
    /// </summary>
    /// <typeparam name="TNew">The type of the new option.</typeparam>
    /// <param name="func">The mapping function.</param>
    /// <returns>
    /// If the option is a value, <paramref name="func"/> applied to value,
    /// <see cref="Option{TNew}.None"/> otherwise.
    /// </returns>
    public Option<TNew> Map<TNew>(Func<T, TNew> func) =>
        this is Some some ? new Option<TNew>.Some(func(some.value)) : Option<TNew>.None;

    /// <summary>
    /// Calls an action if the option is a value. Does nothing otherwise.
    /// </summary>
    /// <param name="action">The action.</param>
    public void Map(Action<T> action)
    {
        if (this is Some some)
        {
            action(some.value);
        }
    }

    /// <summary>
    /// Unwraps the value or throws an exception if the option is not a value.
    /// </summary>
    /// <returns>The value.</returns>
    /// <exception cref="InvalidOperationException">The option is not a value.</exception>
    public T Unwrap() =>
        this is Some some
            ? some.value
            : throw new InvalidOperationException("Invalid option access");

    private Option() { }
}
