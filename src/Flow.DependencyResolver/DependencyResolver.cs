using Flow.DependencyResolver.Internal.Diagnostics;
using Flow.DependencyResolver.Internal.Graph;
using Flow.DependencyResolver.Internal.Pipeline;

namespace Flow.DependencyResolver;

public delegate TKey DependencyKeySelector<T, TKey>(T Item);
public delegate IReadOnlyCollection<Dependency<TKey>> DependencySelector<T, TKey>(T item);

public static class DependencyResolver
{
    internal static DependencyResolutionResult<TKey> Resolve<TKey>(IReadOnlyCollection<DependencyNode<TKey>> nodes) where TKey : notnull
    {
        var failureCollection = new FailureCollection<TKey>();
        var graph = GraphBuilder.Build(nodes, failureCollection);

        CycleDetector.DetectCycles(graph, failureCollection);
        FailurePropagator.Propogate(graph, failureCollection);
        var ordered = TopologicalSorter.Sort(graph, failureCollection);

        return new DependencyResolutionResult<TKey>(ordered, failureCollection);
    }

    public static DependencyResolutionResult<TKey> Resolve<T, TKey>(IReadOnlyCollection<T> items, DependencyKeySelector<T, TKey> getKey, DependencySelector<T, TKey> getDependencies) where TKey : notnull
    {
        IReadOnlyCollection<DependencyNode<TKey>> nodes = items.Select(node => 
        {
            var key = getKey(node);
            var dependencies = getDependencies(node);
            return new DependencyNode<TKey>(key, dependencies);
        }).ToList();

        var failureCollection = new FailureCollection<TKey>();
        var graph = GraphBuilder.Build(nodes, failureCollection);

        CycleDetector.DetectCycles(graph, failureCollection);
        FailurePropagator.Propogate(graph, failureCollection);
        var ordered = TopologicalSorter.Sort(graph, failureCollection);

        return new DependencyResolutionResult<TKey>(ordered, failureCollection);
    }

    public static Builder<T, TKey> From<T, TKey>(IReadOnlyCollection<T> items) where TKey : notnull
    {
        return new Builder<T, TKey>(items);
    }

    public class Builder<T, TKey>(IReadOnlyCollection<T> items) where TKey : notnull
    {
        private readonly IReadOnlyCollection<T> _items = items;
        private DependencyKeySelector<T, TKey>? _keyGetter = null;
        private DependencySelector<T, TKey>? _dependencyGetter = null;

        public Builder<T, TKey> UseKey(DependencyKeySelector<T, TKey> getKey)
        {
            _keyGetter = getKey;
            return this;
        }

        public Builder<T, TKey> DependsOn(DependencySelector<T, TKey> getDependencies)
        {
            _dependencyGetter = getDependencies;
            return this;
        }

        public DependencyResolutionResult<TKey> Resolve()
        {
            if (_keyGetter is null) throw new InvalidOperationException("Key getter isn't set");
            if (_dependencyGetter is null) throw new InvalidOperationException("Dependency getter isn't set");
            return DependencyResolver.Resolve(_items, _keyGetter, _dependencyGetter);
        }
    }
}
