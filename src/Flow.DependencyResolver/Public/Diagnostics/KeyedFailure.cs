namespace Flow.DependencyResolver.Diagnostics;

public class KeyedFailure<TKey> : IFailure where TKey : notnull
{
    public readonly TKey Key;
    public IFailureReason Reason { get; init; }

    internal KeyedFailure(TKey key, IFailureReason reason) => (Key, Reason) = (key, reason);

    public override string ToString() => $"{Key}: {Reason}";
}
