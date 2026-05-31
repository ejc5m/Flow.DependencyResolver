using Flow.DependencyResolver.Internal.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow.DependencyResolver.Public;

public static class DependencyGraph
{
    public static DependencyGraphBuilder<TKey> Create<TKey>() where TKey : notnull => new();

}
public class DependencyGraphBuilder<TKey>() where TKey : notnull
{
    private readonly List<DependencyNode<TKey>> _nodes = [];

    private DependencyNodeBuilder<TKey>? _currentNode;

    public DependencyNodeBuilder<TKey> Add(TKey key)
    {
        _currentNode = new DependencyNodeBuilder<TKey>(key, this);
        _nodes.Add(_currentNode.Node);
        return _currentNode;
    }

    internal DependencyResolutionResult<TKey> Resolve()
    {
        return DependencyResolver.Resolve(_nodes);
    }


}
public class DependencyNodeBuilder<TKey> where TKey : notnull
{
    internal  DependencyNode<TKey> Node;
    private readonly TKey Key;

    List<Dependency<TKey>> temp = [];

    private readonly DependencyGraphBuilder<TKey> _parent;

    internal DependencyNodeBuilder(TKey key, DependencyGraphBuilder<TKey> parent)
    {
        Node = new DependencyNode<TKey>(key, []);
        Key = key;
        _parent = parent;
    }

    public DependencyNodeBuilder<TKey> DependsOn(params TKey[] keys)
    {
        foreach (var key in keys)
            temp.Add(new Dependency<TKey>(key));
        return this;
    }

    public DependencyNodeBuilder<TKey> Optional(params TKey[] keys)
    {
        foreach (var key in keys)
            temp.Add(new Dependency<TKey>(key).Optional());
        return this;
    }

    public DependencyNodeBuilder<TKey> Add(TKey key)
    {
        Node = new(Key, temp);
        return _parent.Add(key);
    }

    public DependencyResolutionResult<TKey> Resolve() => _parent.Resolve();
}