namespace Flow.DependencyResolver.Diagnostics;

public sealed record InvalidDependencyFailure<TKey>(TKey Dependency) : IFailureReason
{
    public override string ToString() => $"Required dependency '{Dependency}' is invalid.";
}