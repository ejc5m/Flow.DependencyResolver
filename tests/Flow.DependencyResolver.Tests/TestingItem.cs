using System.Diagnostics.CodeAnalysis;

namespace Flow.DependencyResolver.Tests;

//public record TestingItem(string Name, Dependency<string>[] Dependencies);

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
public record TestingItem2();