namespace Utils;

public record Result<TOk, TErr>
{
    public sealed record Ok(TOk value) : Result<TOk, TErr>;

    public sealed record Err(TErr value) : Result<TOk, TErr>;

    public TOk UnwrapOrElse(Func<TErr, TOk> func) =>
        this is Ok ok ? ok.value : func((this as Err)!.value);

    public bool IsOk() => this is Ok;

    public bool IsErr() => this is Err;

    public TErr UnwrapErr() =>
        this is Err err ? err.value : throw new InvalidOperationException("Invalid result access");

    private Result() { }
}
