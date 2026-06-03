using Flow.DependencyResolver.Diagnostics;
using Flow.DependencyResolver.Internal.Diagnostics;
using Flow.DependencyResolver.Internal.Graph;

namespace Flow.DependencyResolver.Internal.Pipeline;

internal static class FailurePropagator
{
    internal static void Propogate<TKey>(Graph<TKey> graph, FailureCollection<TKey> failureCollection) where TKey : notnull
    {
        var queue = new Queue<TKey>(failureCollection.FailedKeys);

        while (queue.Count > 0)
        {
            TKey invalidNode = queue.Dequeue();

            foreach (var dependent in graph.Reverse[invalidNode])
            {
                if (AreInSameCycle(failureCollection, dependent, invalidNode)) continue;

                if (!failureCollection.HasFailures(dependent))
                    queue.Enqueue(dependent);

                failureCollection.AddFailureReason(dependent, new InvalidDependencyFailure<TKey>(invalidNode));
            }
        }
    }

    private static bool AreInSameCycle<TKey>(FailureCollection<TKey> failureCollection, TKey a, TKey b) where TKey : notnull
    {
        if (!failureCollection.FailuresByKey.TryGetValue(a, out var failures))
            return false;

        foreach (var failure in failures)
            if (failure.Reason is PartOfACycleFailure<TKey> cycle && cycle.NodesInCycle.Contains(b))
                return true;

        return false;
    }
}
