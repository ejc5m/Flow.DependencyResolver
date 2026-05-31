namespace Flow.DependencyResolver.Diagnostics;

public record DependencyCycle<TKey>(IReadOnlyList<TKey> NodesInCycle)
{
    public static DependencyCycle<TKey> Create(IReadOnlyList<TKey> nodesInCycle) => new(nodesInCycle);

    public override string ToString()
    {
        if (NodesInCycle.Count is 0) return String.Empty;
        return $"{string.Join(" -> ", NodesInCycle)} -> {NodesInCycle[0]}";
    }
}
