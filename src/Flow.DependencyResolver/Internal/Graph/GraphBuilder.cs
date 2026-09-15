using Flow.DependencyResolver.Diagnostics;
using Flow.DependencyResolver.Internal.Diagnostics;

namespace Flow.DependencyResolver.Internal.Graph;

internal static class GraphBuilder
{
    internal static Graph<TKey> Build<TKey>(IReadOnlyCollection<DependencyNode<TKey>> nodes, FailureCollection<TKey> failureCollection, IEqualityComparer<TKey> comparer) where TKey : notnull
    {
        Graph<TKey> graph = new(nodes.Count, comparer);
        HashSet<TKey> duplicates = new(comparer);

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

        HashSet<TKey> knownKeys = new(nodes.Select(node => node.Key).Where(key => !duplicates.Contains(key)), comparer);

        foreach (var node in nodes)
        {
            if (duplicates.Contains(node.Key))
                continue;

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

                TKey source = dependency.Direction == DependencyDirection.After ? dependency.Key : node.Key;
                TKey target = dependency.Direction == DependencyDirection.After ? node.Key : dependency.Key;

                //Ignore duplicate dependencies, e.g if Item1 depends on Item2 and Item2
                if (!graph.Forward[target].Contains(source, comparer))
                {
                    graph.Forward[target].Add(source);
                    graph.Reverse[source].Add(target);
                }
            }
        }

        return graph;
    }
}
