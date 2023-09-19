namespace Utils;

public record Result<TOk, TErr>
{
    public sealed record Ok(TOk value) : Result<TOk, TErr>;
    public sealed record Err(TErr value) : Result<TOk, TErr>;

    public TOk UnwrapOrElse(Func<TErr, TOk> func)
        => this is Ok ok ? ok.value : func((this as Err)!.value);

    private Result() { }
}
