namespace Flow.DependencyResolver.Tests;

//public record TestingItem(string Name, Dependency<string>[] Dependencies);

public class TestingItem
{
    public required string Name;
    public Dependency<string>[] Dependencies = [];
}
public record TestingItem2();