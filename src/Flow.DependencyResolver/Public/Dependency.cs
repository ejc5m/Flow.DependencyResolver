namespace Flow.DependencyResolver;

public struct Dependency<TKey>
{
    public readonly TKey Key;
    public bool IsOptional { get; private set; } = false;
    public DependencyDirection Direction = DependencyDirection.After;

    public Dependency(TKey key, bool isOptional = false, DependencyDirection direction = DependencyDirection.After)
    {
        Key = key;
        IsOptional = isOptional;
        Direction = direction;
    }

    public Dependency<TKey> Optional()
    {
        IsOptional = true;
        return this;
    }

    public Dependency<TKey> Before()
    {
        Direction = DependencyDirection.Before;
        return this;
    }
}