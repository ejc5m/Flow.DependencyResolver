using Flow.DependencyResolver.Diagnostics;

namespace Flow.DependencyResolver.Internal.Diagnostics;

internal sealed class FailureCollection<TKey> : IReadOnlyFailureCollection<TKey> where TKey : notnull
{
    private readonly List<IFailureReason> _globalFailures = []; 
    private readonly Dictionary<TKey, List<IFailureReason>> _failureReasons = [];

    public IReadOnlyList<TKey> FailedKeys => _failureReasons.Keys.ToList();

    public bool HasFailures(TKey key) => _failureReasons.ContainsKey(key);

    internal void AddFailureReason(TKey key, IFailureReason reason)
    {
        if (!_failureReasons.TryGetValue(key, out var failReasons))
        {
            failReasons = [];
            _failureReasons[key] = failReasons;
        }

        failReasons.Add(reason);
    }

    internal void AddGlobalFailure(IFailureReason reason) => _globalFailures.Add(reason);

    public IReadOnlyList<IFailureReason> GetGlobalFailures() => _globalFailures;

    public IReadOnlyList<IFailureReason> GetFailureReasonsOfKey(TKey key)
    {
        return _failureReasons.TryGetValue(key, out var failReasons) ? failReasons : Array.Empty<IFailureReason>();
    }

    public IReadOnlyList<Failure<TKey>> GetAllFailures()
    {
        List<Failure<TKey>> reasons = [];
        foreach (var (key, failReasons) in _failureReasons)
        {
            foreach (var failReason in failReasons)
            {
                reasons.Add(new Failure<TKey>(key, failReason));
            }
        }
        return reasons;
    }
}
