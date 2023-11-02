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
    /// Maps the result to a new result.
    /// </summary>
    /// <typeparam name="TNew">The type of the new result.</typeparam>
    /// <param name="func">The mapping function.</param>
    /// <returns>
    /// If the result is a value, <paramref name="func"/> applied to value,
    /// containing the error otherwise.
    /// </returns>
    public Result<TNew, TErr> Map<TNew>(Func<TOk, TNew> func) =>
        this is Ok ok
            ? new Result<TNew, TErr>.Ok(func(ok.value))
            : new Result<TNew, TErr>.Err((this as Err)!.value);

    public TNew MapOrElse<TNew>(Func<TOk, TNew> funcOk, Func<TErr, TNew> funcErr) =>
        this is Ok ok ? funcOk(ok.value) : funcErr((this as Err)!.value);

    /// <summary>
    /// Tries to unwrap the value.
    /// </summary>
    /// <returns>Unwrapped value or <see cref="Option{T}.None"/> if this is <see cref="Result{TOk, TErr}.Err"/>.</returns>
    public Option<TOk> TryUnwrap() => this is Ok ok ? ok.value : Option<TOk>.None;

    /// <summary>
    /// Unwraps the error or throws an exception if the result is a value.
    /// </summary>
    /// <returns>The error.</returns>
    /// <exception cref="InvalidOperationException">If the result is a value.</exception>
    public TErr UnwrapErr() =>
        this is Err err ? err.value : throw new InvalidOperationException("Invalid result access");

    private Result() { }
}
