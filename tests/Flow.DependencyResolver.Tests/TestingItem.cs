using System.Diagnostics.CodeAnalysis;

namespace Flow.DependencyResolver.Tests;

public class TestingItem
{
    public required string Name;
    public Dependency<string>[] Dependencies = [];

    [SetsRequiredMembers]
    public TestingItem(string name, Dependency<string>[] dependencies)
    {
        Name = name;
        Dependencies = dependencies;
    }

    public TestingItem()
    {

    }
}