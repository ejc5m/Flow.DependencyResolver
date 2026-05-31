using Flow.DependencyResolver.Internal.Diagnostics;
using Flow.DependencyResolver.Internal.Graph;

namespace Flow.DependencyResolver.Internal.Pipeline;

internal static class TopologicalSorter
{
    internal static IReadOnlyCollection<TKey> Sort<TKey>(Graph<TKey> graph, FailureCollection<TKey> failureCollection) where TKey : notnull
    {
        var inDegree = new Dictionary<TKey, int>();

        foreach (var node in graph.Forward.Keys)
        {
            if (!failureCollection.HasFailures(node))
                inDegree[node] = 0;
        }

        foreach (var (key, dependencies) in graph.Forward)
        {
            if (failureCollection.HasFailures(key)) continue;

            foreach (var dependency in dependencies)
            {
                if (failureCollection.HasFailures(dependency)) continue;

                inDegree[key]++;
            }
        }

        var queue = new Queue<TKey>(inDegree.Where(static x => x.Value == 0).Select(static x => x.Key));

        var sorted = new List<TKey>();

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            sorted.Add(node);

            foreach (var dependent in graph.Reverse[node])
            {
                if (failureCollection.HasFailures(node)) continue;

                if (--inDegree[dependent] == 0)
                    queue.Enqueue(dependent);
            }
        }

        return sorted;
    }
}
