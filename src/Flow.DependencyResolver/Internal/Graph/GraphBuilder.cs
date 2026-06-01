using Flow.DependencyResolver.Diagnostics;
using Flow.DependencyResolver.Internal.Diagnostics;

namespace Flow.DependencyResolver.Internal.Graph;

internal static class GraphBuilder
{
    internal static Graph<TKey> Build<TKey>(IReadOnlyCollection<DependencyNode<TKey>> nodes, FailureCollection<TKey> failureCollection) where TKey : notnull
    {
        Graph<TKey> graph = new();

        foreach (var node in nodes)
        {
            if (graph.Forward.ContainsKey(node.Key))
            {
                failureCollection.AddGlobalFailure(new DuplicateKeyFailure<TKey>(node.Key));
                continue;
            }

            graph.Forward[node.Key] = [];
            graph.Reverse[node.Key] = [];
        }

        HashSet<TKey> knownKeys = nodes.Select(x => x.Key).ToHashSet();

        foreach (var node in nodes)
        {
            foreach (var dependency in node.Dependencies)
            {
                if (!knownKeys.Contains(dependency.Key))
                {
                    if (dependency.IsOptional)
                        continue;

                    failureCollection.AddFailureReason(node.Key, new MissingDependencyFailure<TKey>(dependency.Key));

                    continue;
                }

                graph.Forward[node.Key].Add(dependency.Key);
                graph.Reverse[dependency.Key].Add(node.Key);
            }
        }

        return graph;
    }
}
