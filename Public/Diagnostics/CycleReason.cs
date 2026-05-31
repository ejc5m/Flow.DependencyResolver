namespace Flow.DependencyResolver.Diagnostics;

public sealed class CycleReason<TKey> : IFailureReason
{
    public DependencyCycle<TKey> Cycle;

    public CycleReason(DependencyCycle<TKey> cycle) => Cycle = cycle;
    public CycleReason(IReadOnlyList<TKey> cycle) => Cycle = DependencyCycle<TKey>.Create(cycle);
}
