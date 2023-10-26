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
    /// Creates a new option from a nullable value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    /// A new option if <paramref name="value"/> is not null,
    /// <see cref="Option{T}.None"/> otherwise.
    /// </returns>
    public static Option<T> From(T? value) => value != null ? new Some(value) : None;

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
    /// Checks if the option is no value.
    /// </summary>
    /// <returns><c>true</c> if the option is no value, <c>false</c> otherwise.</returns>
    public bool IsNone() => this is _None;

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
    /// Maps the option to a new option.
    /// </summary>
    /// <typeparam name="TNew">The type of the new option.</typeparam>
    /// <param name="func">The mapping function.</param>
    /// <param name="onNone">The value to return if the option is no value.</param>
    /// <returns>
    /// If the option is a value, <paramref name="func"/> applied to value,
    /// <paramref name="onNone"/> otherwise.
    /// </returns>
    public TNew MapOr<TNew>(Func<T, TNew> func, TNew onNone) =>
        this is Some some ? func(some.value) : onNone;

    /// <summary>
    /// Maps the option to a new option.
    /// </summary>
    /// <typeparam name="TNew">The type of the new option.</typeparam>
    /// <param name="func">The mapping function.</param>
    /// <param name="onNone">The value to return if the option is no value.</param>
    /// <returns>
    /// If the option is a value, <paramref name="func"/> applied to value,
    /// <paramref name="onNone"/> otherwise.
    /// </returns>
    public async Task<TNew> MapOr<TNew>(Func<T, Task<TNew>> func, TNew onNone) =>
        this is Some some ? await func(some.value) : onNone;

    /// <summary>
    /// Maps the option to a new option.
    /// </summary>
    /// <typeparam name="TNew">The type of the new option.</typeparam>
    /// <param name="funcSome">The mapping function.</param>
    /// <param name="funcNone">The function to call if the option is no value.</param>
    /// <returns>
    /// If the option is a value, <paramref name="funcSome"/> applied to value,
    /// <paramref name="funcNone"/> applied otherwise.
    /// </returns>
    public TNew MapOrElse<TNew>(Func<T, TNew> funcSome, Func<TNew> funcNone) =>
        this is Some some ? funcSome(some.value) : funcNone();

    /// <summary>
    /// Calls an action if the option is a value. Calls another action otherwise.
    /// </summary>
    /// <param name="actionSome">The action to apply on some.</param>
    /// <param name="actionNone">The action to call on none.</param>
    public void MapOrElse(Action<T> actionSome, Action actionNone)
    {
        if (this is Some some)
        {
            actionSome(some.value);
        }
        else
        {
            actionNone();
        }
    }

    /// <summary>
    /// Maps the option to a new option.
    /// </summary>
    /// <typeparam name="TNew">The type of the new option.</typeparam>
    /// <param name="func">The mapping function.</param>
    /// <returns>
    /// If the option is a value, <paramref name="func"/> applied to value,
    /// <see cref="Option{TNew}.None"/> otherwise.
    /// </returns>
    public Option<TNew> AndThen<TNew>(Func<T, Option<TNew>> func) =>
        this is Some some ? func(some.value) : Option<TNew>.None;

    /// <summary>
    /// Unwraps the value or throws an exception if the option is not a value.
    /// </summary>
    /// <returns>The value.</returns>
    /// <exception cref="InvalidOperationException">The option is not a value.</exception>
    public T Unwrap() =>
        this is Some some
            ? some.value
            : throw new InvalidOperationException("Invalid option access");

    /// <summary>
    /// Tries all values in a list of options.
    /// </summary>
    /// <param name="list">The list.</param>
    /// <returns>
    /// A list of all values in <paramref name="list"/> if all options are values,
    /// <see cref="Option{T}.None"/> otherwise.
    /// </returns>
    public static Option<IEnumerable<T>> Collect(IEnumerable<Option<T>> list) =>
        list.Aggregate(
                new Option<List<T>>.Some(new List<T>()),
                (Option<List<T>> acc, Option<T> x) =>
                    acc.AndThen<List<T>>(
                        (List<T> list) =>
                            x.Map(value =>
                            {
                                list.Add(value);
                                return list;
                            })
                    )
            )
            .AndThen(x => new Option<IEnumerable<T>>.Some(x));

    private Option() { }
}
