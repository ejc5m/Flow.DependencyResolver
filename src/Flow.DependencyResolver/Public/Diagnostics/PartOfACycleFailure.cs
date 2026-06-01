namespace Flow.DependencyResolver.Diagnostics;

public sealed class PartOfACycleFailure<TKey> : IFailureReason
{
    public DependencyCycle<TKey> Cycle;

    public PartOfACycleFailure(DependencyCycle<TKey> cycle) => Cycle = cycle;
    public PartOfACycleFailure(IReadOnlyList<TKey> cycle) => Cycle = DependencyCycle<TKey>.Create(cycle);
}
