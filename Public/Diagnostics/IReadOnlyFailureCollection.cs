namespace Flow.DependencyResolver.Diagnostics;

public interface IReadOnlyFailureCollection<TKey> where TKey : notnull
{
    public IReadOnlyList<IFailureReason> GetFailureReasons(TKey key);
    public bool HasFailures(TKey key);
    public IReadOnlyList<TKey> FailedKeys { get; }
}
