using Flow.DependencyResolver.Diagnostics;
using Flow.DependencyResolver.Internal.Diagnostics;
using Flow.DependencyResolver.Internal.Graph;

namespace Flow.DependencyResolver.Internal.Pipeline;

internal static class CycleDetector
{
    internal static void DetectCycles<TKey>(Graph<TKey> graph, FailureCollection<TKey> failureCollection) where TKey : notnull
    {
        var visited = new HashSet<TKey>();
        var recStack = new HashSet<TKey>();
        var path = new List<TKey>();

        foreach (var node in graph.Forward.Keys)
        {
            if (!visited.Contains(node))
            {
                DFS(node, graph, failureCollection, visited, recStack, path);
            }
        }
    }

    private static void DFS<TKey>(TKey current, Graph<TKey> graph, FailureCollection<TKey> failureCollection, HashSet<TKey> visited, HashSet<TKey> recStack, List<TKey> path) where TKey : notnull
    {
        visited.Add(current);
        recStack.Add(current);
        path.Add(current);

        foreach (var next in graph.Forward[current])
        {
            if (!visited.Contains(next))
            {
                DFS(next, graph, failureCollection, visited, recStack, path);
            }
            else if (recStack.Contains(next))
            {
                //Cycle detected
                int index = path.IndexOf(next);
                var cycle = path.Skip(index).ToList();

                foreach (var nodeInCycle in cycle)
                {
                    failureCollection.AddFailureReason(nodeInCycle, new PartOfACycleFailure<TKey>(cycle));
                }
            }
        }

        recStack.Remove(current);
        path.RemoveAt(path.Count - 1);
    }
}
