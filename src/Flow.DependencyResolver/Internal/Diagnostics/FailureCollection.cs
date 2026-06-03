using Flow.DependencyResolver.Diagnostics;

namespace Flow.DependencyResolver.Internal.Diagnostics;

internal sealed class FailureCollection<TKey> : IReadOnlyFailureCollection<TKey> where TKey : notnull
{
    private readonly List<Failure> _globalFailures = []; 
    private readonly Dictionary<TKey, List<KeyedFailure<TKey>>> _failuresByKey = [];

    //Public API
    public IReadOnlyList<Failure> GlobalFailures => _globalFailures;

    public IReadOnlyDictionary<TKey, IReadOnlyList<KeyedFailure<TKey>>> FailuresByKey =>
        _failuresByKey.ToDictionary(x => x.Key, x => (IReadOnlyList<KeyedFailure<TKey>>)x.Value.AsReadOnly()).AsReadOnly();

    public IEnumerable<IFailure> EnumerateFailures()
    {
        foreach (var failure in GlobalFailures)
            yield return failure;

        foreach (var (key, failures) in FailuresByKey)
            foreach (var failure in failures)
                yield return failure;
    }

    //Internal helper methods
    internal IReadOnlySet<TKey> FailedKeys => _failuresByKey.Keys.ToHashSet();
    internal bool HasFailures(TKey key) => _failuresByKey.ContainsKey(key);

    internal void AddFailureReason(TKey key, IFailureReason reason)
    {
        if (!_failuresByKey.TryGetValue(key, out var failReasons))
        {
            failReasons = [];
            _failuresByKey[key] = failReasons;
        }

        failReasons.Add(new(key, reason));
    }

    internal void AddGlobalFailure(IFailureReason reason) => _globalFailures.Add(new(reason));
}
