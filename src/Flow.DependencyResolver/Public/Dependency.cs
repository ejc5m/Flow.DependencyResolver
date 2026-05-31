namespace Flow.DependencyResolver;

public struct Dependency<TKey>
{
    public readonly TKey Key;
    public bool IsOptional = false;

    public Dependency(TKey key, bool isOptional = false)
    {
        Key = key;
        IsOptional = isOptional;
    }

    public Dependency<TKey> Optional()
    {
        IsOptional = true;
        return this;
    }
}