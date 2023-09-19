namespace Utils;

public static class Try<TException>
where TException : Exception
{
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
}
