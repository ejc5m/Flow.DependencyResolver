namespace Flow.DependencyResolver.Diagnostics;

public sealed record PartOfACycleFailure<TKey>(IReadOnlyList<TKey> NodesInCycle) : IFailureReason
{
    public override string ToString()
    {
        string cycleText = string.Empty;
        if (NodesInCycle.Count is not 0)
            cycleText = $"{string.Join(" -> ", NodesInCycle)} -> {NodesInCycle[0]}";

        return $"Part of cycle '{cycleText}'.";
    }
}
