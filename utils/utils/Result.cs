namespace Utils;

/// <summary>
/// Represents a value that can be either a value or an error.
/// </summary>
/// <typeparam name="TOk">The type of the value.</typeparam>
/// <typeparam name="TErr">The type of the error.</typeparam>
public record Result<TOk, TErr>
{
    /// <summary>
    /// Represents a value.
    /// </summary>
    /// <param name="value">The value.</param>
    public sealed record Ok(TOk value) : Result<TOk, TErr>;

    /// <summary>
    /// Represents an error.
    /// </summary>
    /// <param name="value">The error.</param>
    public sealed record Err(TErr value) : Result<TOk, TErr>;

    /// <summary>
    /// Unwraps the value or calls a function if the result is an error.
    /// </summary>
    /// <param name="func">The function.</param>
    /// <returns>
    /// The value if the result is a value,
    /// <paramref name="func"/> applied to the error otherwise.
    /// </returns>
    public TOk UnwrapOrElse(Func<TErr, TOk> func) =>
        this is Ok ok ? ok.value : func((this as Err)!.value);

    /// <summary>
    /// Checks if the result is a value.
    /// </summary>
    /// <returns><c>true</c> if the result is a value, <c>false</c> otherwise.</returns>
    public bool IsOk() => this is Ok;

    /// <summary>
    /// Checks if the result is an error.
    /// </summary>
    /// <returns><c>true</c> if the result is an error, <c>false</c> otherwise.</returns>
    public bool IsErr() => this is Err;

    /// <summary>
    /// Unwraps the error or throws an exception if the result is a value.
    /// </summary>
    /// <returns>The error.</returns>
    /// <exception cref="InvalidOperationException">If the result is a value.</exception>
    public TErr UnwrapErr() =>
        this is Err err ? err.value : throw new InvalidOperationException("Invalid result access");

    private Result() { }
}
