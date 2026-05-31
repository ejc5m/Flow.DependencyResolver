using Flow.DependencyResolver.Diagnostics;

namespace Flow.DependencyResolver;

public record DependencyResolutionResult<TKey>(IReadOnlyCollection<TKey> Ordered, IReadOnlyFailureCollection<TKey> Failures) where TKey : notnull;