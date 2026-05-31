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
                if (!failureCollection.HasFailures(dependent))
                    queue.Enqueue(dependent);

                failureCollection.AddFailureReason(dependent, new DependsOnInvalidReason<TKey>(invalidNode));
            }
        }
    }
}
