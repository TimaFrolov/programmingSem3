namespace Utils;

/// <summary>
/// Class for trying to call a function that may throw an exception.
/// </summary>
/// <typeparam name="TException">The type of the exception.</typeparam>
public static class Try<TException>
where TException : Exception
{
    /// <summary>
    /// Calls a function that may throw an exception.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="func">The function.</param>
    /// <returns>
    /// If the function throws an exception, <see cref="Result{TResult, TException}.Err"/> with the exception,
    /// <see cref="Result{TResult, TException}.Ok"/> with the result otherwise.
    /// </returns>
    public static Result<TResult, TException> Call<TResult>(Func<TResult> func)
    {
        try
        {
            return new Result<TResult, TException>.Ok(func());
        }
        catch (TException exception)
        {
            return new Result<TResult, TException>.Err(exception);
        }
    }

    /// <summary>
    /// Calls an async function that may throw an exception.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="func">The function.</param>
    /// <returns>
    /// If the function throws an exception, <see cref="Result{TResult, TException}.Err"/> with the exception,
    /// <see cref="Result{TResult, TException}.Ok"/> with the result otherwise.
    /// </returns>
    public static async Task<Result<TResult, TException>> CallAsync<TResult>(
        Func<Task<TResult>> func
    )
    {
        try
        {
            return new Result<TResult, TException>.Ok(await func());
        }
        catch (TException exception)
        {
            return new Result<TResult, TException>.Err(exception);
        }
    }

    public static async Task<Option<TException>> CallAsync(
        Func<Task> func
    )
    {
        try
        {
            await func();
            return Option<TException>.None;
        }
        catch (TException exception)
        {
            return new Option<TException>.Some(exception);
        }
    }
}
