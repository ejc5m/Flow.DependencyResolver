using Flow.DependencyResolver.Diagnostics;

namespace Flow.DependencyResolver.Internal.Diagnostics;

internal sealed class FailureCollection<TKey> : IReadOnlyFailureCollection<TKey> where TKey : notnull
{
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

    public IReadOnlyList<IFailureReason> GetFailureReasons(TKey key)
    {
        return _failureReasons.TryGetValue(key, out var failReasons) ? failReasons : Array.Empty<IFailureReason>();
    }
}
