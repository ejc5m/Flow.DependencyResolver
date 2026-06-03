namespace Flow.DependencyResolver.Diagnostics;

public interface IReadOnlyFailureCollection<TKey> where TKey : notnull
{
    public IReadOnlyList<Failure> GlobalFailures { get; }
    public IReadOnlyDictionary<TKey, IReadOnlyList<KeyedFailure<TKey>>> FailuresByKey { get; }
    public IEnumerable<IFailure> EnumerateFailures();
}
