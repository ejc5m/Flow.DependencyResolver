namespace Flow.DependencyResolver;

public class DependencyNodeBuilder<TKey> where TKey : notnull
{
    private readonly TKey _key;

    private readonly List<Dependency<TKey>> _node = [];

    private readonly DependencyGraphBuilder<TKey> _parent;

    internal DependencyNodeBuilder(TKey key, DependencyGraphBuilder<TKey> parent)
    {
        _key = key;
        _parent = parent;
    }

    public DependencyNodeBuilder<TKey> DependsOn(params TKey[] keys)
    {
        foreach (var key in keys)
            _node.Add(new Dependency<TKey>(key));
        return this;
    }

    public DependencyNodeBuilder<TKey> OptionallyDependsOn(params TKey[] keys)
    {
        foreach (var key in keys)
            _node.Add(new Dependency<TKey>(key).Optional());
        return this;
    }

    public DependencyNodeBuilder<TKey> IsDependedOnBy(params TKey[] keys)
    {
        foreach (var key in keys)
            _node.Add(new Dependency<TKey>(key).Before());
        return this;
    }

    public DependencyNodeBuilder<TKey> OptionallyIsDependedOnBy(params TKey[] keys)
    {
        foreach (var key in keys)
            _node.Add(new Dependency<TKey>(key).Optional().Before());
        return this;
    }

    public DependencyNodeBuilder<TKey> Add(TKey key)
    {
        _parent.Nodes.Add(new(_key, _node));
        return _parent.Add(key);
    }

    public DependencyResolutionResult<TKey> Resolve(IEqualityComparer<TKey>? comparer = null)
    {
        _parent.Nodes.Add(new(_key, _node));
        return _parent.Resolve(comparer);
    }
}
