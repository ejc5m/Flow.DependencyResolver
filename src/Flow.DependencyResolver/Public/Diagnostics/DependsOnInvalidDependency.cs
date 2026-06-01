namespace Flow.DependencyResolver.Diagnostics;

public sealed record DependsOnInvalidDependency<TKey>(TKey Dependency) : IFailureReason;