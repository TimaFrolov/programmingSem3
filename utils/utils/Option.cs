namespace Utils;

public record Option<T>
{
    public sealed record Some(T value) : Option<T>;
    public sealed record _None : Option<T>;

    public static readonly _None None = new _None();

    public static implicit operator Option<T>(T value) => new Some(value);

    public bool IsSome() => this is Some;

    public Option<TNew> Map<TNew>(Func<T, TNew> func)
        => this is Some some ? new Option<TNew>.Some(func(some.value)) : Option<TNew>.None;

    public void Map(Action<T> action)
    {
        if (this is Some some)
        {
            action(some.value);
        }
    }

    public T Unwrap()
        => this is Some some ? some.value : throw new InvalidOperationException("Invalid result access");

    private Option() { }
}
