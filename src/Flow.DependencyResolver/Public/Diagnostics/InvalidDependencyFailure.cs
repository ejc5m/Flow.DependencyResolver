namespace Flow.DependencyResolver.Diagnostics;

public sealed record InvalidDependencyFailure<TKey>(TKey Dependency) : IFailureReason
{
    public override string ToString() => $"Required dependency '{Dependency}' is not usable because it failed for one or more reasons.";
}