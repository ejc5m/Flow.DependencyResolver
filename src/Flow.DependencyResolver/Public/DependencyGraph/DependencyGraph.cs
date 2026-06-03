namespace Flow.DependencyResolver;

public static class DependencyGraph
{
    public static DependencyGraphBuilder<TKey> Create<TKey>() where TKey : notnull => new();
}

