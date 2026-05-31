namespace Flow.DependencyResolver.Diagnostics;

public sealed record DependsOnMissingDependency<TKey>(TKey MissingKey) : IFailureReason;
