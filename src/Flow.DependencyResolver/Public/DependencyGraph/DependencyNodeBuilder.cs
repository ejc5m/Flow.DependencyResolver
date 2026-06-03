namespace Flow.DependencyResolver;

public class DependencyNodeBuilder<TKey> where TKey : notnull
{
    private readonly TKey Key;

    List<Dependency<TKey>> Node = [];

    private readonly DependencyGraphBuilder<TKey> _parent;

    internal DependencyNodeBuilder(TKey key, DependencyGraphBuilder<TKey> parent)
    {
        Key = key;
        _parent = parent;
    }

    public DependencyNodeBuilder<TKey> DependsOn(params TKey[] keys)
    {
        foreach (var key in keys)
            Node.Add(new Dependency<TKey>(key));
        return this;
    }

    public DependencyNodeBuilder<TKey> OptionallyDependsOn(params TKey[] keys)
    {
        foreach (var key in keys)
            Node.Add(new Dependency<TKey>(key).Optional());
        return this;
    }

    public DependencyNodeBuilder<TKey> Add(TKey key)
    {
        _parent.Nodes.Add(new(Key, Node));
        return _parent.Add(key);
    }

    public DependencyResolutionResult<TKey> Resolve()
    {
        _parent.Nodes.Add(new(Key, Node));
        return _parent.Resolve();
    }
}
