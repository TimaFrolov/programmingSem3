public interface IMyTask<T>
{
    public bool IsCompleted { get; }
    public T Result { get; }
    public IMyTask<TNew> ContinueWith<TNew>(Func<T, TNew> continuation);
}
