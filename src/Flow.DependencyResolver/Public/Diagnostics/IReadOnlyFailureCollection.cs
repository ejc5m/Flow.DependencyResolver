namespace Flow.DependencyResolver.Diagnostics;

public interface IReadOnlyFailureCollection<TKey> where TKey : notnull
{
    public IReadOnlyList<TKey> FailedKeys { get; }
    public bool HasFailures(TKey key);
    public IReadOnlyList<IFailureReason> GetGlobalFailures();
    public IReadOnlyList<IFailureReason> GetFailureReasons(TKey key);
}
