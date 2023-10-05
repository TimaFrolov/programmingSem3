public interface IMyTask<T>
{
    public bool IsCompleted { get; }
    public T Result { get; }
    public void Wait();
    public IMyTask<TNew> ContinueWith<TNew>(Func<T, TNew> continuation);
}

public interface IMyTask : IMyTask<Monostate>
{
    public IMyTask<TNew> ContinueWith<TNew>(Func<TNew> continuation);
}
