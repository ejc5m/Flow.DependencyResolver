namespace Flow.DependencyResolver.Diagnostics;

public sealed record DependsOnInvalidReason<TKey>(TKey Dependency) : IFailureReason;