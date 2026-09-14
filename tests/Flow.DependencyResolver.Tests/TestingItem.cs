using System.Diagnostics.CodeAnalysis;

namespace Flow.DependencyResolver.Tests;

public class TestingItem<T>
{
    public required T Key;
    public Dependency<T>[] Dependencies = [];

    [SetsRequiredMembers]
    public TestingItem(T key, Dependency<T>[] dependencies)
    {
        Key = key;
        Dependencies = dependencies;
    }

    public TestingItem()
    {

    }
}