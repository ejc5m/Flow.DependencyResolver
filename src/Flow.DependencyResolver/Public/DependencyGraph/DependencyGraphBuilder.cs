using Flow.DependencyResolver.Internal.Graph;

namespace Flow.DependencyResolver;

public class DependencyGraphBuilder<TKey> where TKey : notnull
{
    internal readonly List<DependencyNode<TKey>> Nodes = [];

    private DependencyNodeBuilder<TKey>? _currentNode;

    internal DependencyGraphBuilder() { }

    public DependencyNodeBuilder<TKey> Add(TKey key)
    {
        _currentNode = new DependencyNodeBuilder<TKey>(key, this);
        return _currentNode;
    }

    internal DependencyResolutionResult<TKey> Resolve() => DependencyResolver.Resolve(Nodes);
}


