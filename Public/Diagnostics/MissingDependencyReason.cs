namespace Flow.DependencyResolver.Diagnostics;

public sealed record MissingDependencyReason<TKey>(TKey MissingKey) : IFailureReason;
