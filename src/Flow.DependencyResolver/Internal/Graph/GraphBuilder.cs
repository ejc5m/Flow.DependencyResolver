using Flow.DependencyResolver.Diagnostics;
using Flow.DependencyResolver.Internal.Diagnostics;

namespace Flow.DependencyResolver.Internal.Graph;

internal static class GraphBuilder
{
    internal static Graph<TKey> Build<TKey>(IReadOnlyCollection<DependencyNode<TKey>> nodes, FailureCollection<TKey> failureCollection) where TKey : notnull
    {
        Graph<TKey> graph = new();
        HashSet<TKey> duplicates = [];

        foreach (var node in nodes)
        {
            if (graph.Forward.ContainsKey(node.Key))
            {
                duplicates.Add(node.Key);
                failureCollection.AddGlobalFailure(new DuplicateKeyFailure<TKey>(node.Key));

                //Remove this key from the graph or else the topological sorter will think it's valid
                graph.Forward.Remove(node.Key);
                graph.Reverse.Remove(node.Key);

                continue;
            }

            graph.Forward[node.Key] = [];
            graph.Reverse[node.Key] = [];
        }

        HashSet<TKey> knownKeys = nodes.Select(node => node.Key).Where(key => !duplicates.Contains(key)).ToHashSet();

        foreach (var node in nodes)
        {
            foreach (var dependency in node.Dependencies)
            {
                if (duplicates.Contains(dependency.Key))
                {
                    failureCollection.AddFailureReason(node.Key, new InvalidDependencyFailure<TKey>(dependency.Key));
                    continue;
                }

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
