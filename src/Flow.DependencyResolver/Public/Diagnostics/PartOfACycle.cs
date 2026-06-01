namespace Flow.DependencyResolver.Diagnostics;

public sealed class PartOfACycle<TKey> : IFailureReason
{
    public DependencyCycle<TKey> Cycle;

    public PartOfACycle(DependencyCycle<TKey> cycle) => Cycle = cycle;
    public PartOfACycle(IReadOnlyList<TKey> cycle) => Cycle = DependencyCycle<TKey>.Create(cycle);
}
