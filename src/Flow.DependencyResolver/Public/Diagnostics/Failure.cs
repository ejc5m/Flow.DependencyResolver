namespace Flow.DependencyResolver.Diagnostics;

public record Failure<TKey>(TKey Key, IFailureReason Reason)
{
    public override string ToString() => $"{Key}: {Reason}";
}