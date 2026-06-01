namespace Flow.DependencyResolver.Diagnostics;

public record DuplicateKeyFailure<TKey>(TKey Duplicate) : IFailureReason;