namespace Flow.DependencyResolver.Diagnostics;

public sealed record DependsOnMissingDependencyFailure<TKey>(TKey MissingKey) : IFailureReason;
