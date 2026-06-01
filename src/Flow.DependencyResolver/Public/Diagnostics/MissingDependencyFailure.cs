namespace Flow.DependencyResolver.Diagnostics;

public sealed record MissingDependencyFailure<TKey>(TKey MissingKey) : IFailureReason
{
    public override string ToString() => $"Missing dependency '{MissingKey}'.";
}
