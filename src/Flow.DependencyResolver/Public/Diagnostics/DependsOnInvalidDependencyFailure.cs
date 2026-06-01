namespace Flow.DependencyResolver.Diagnostics;

public sealed record DependsOnInvalidDependencyFailure<TKey>(TKey Dependency) : IFailureReason;