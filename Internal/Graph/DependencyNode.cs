namespace Flow.DependencyResolver.Internal.Graph;

internal sealed record DependencyNode<TKey>(TKey Key, IReadOnlyCollection<Dependency<TKey>> Dependencies);
