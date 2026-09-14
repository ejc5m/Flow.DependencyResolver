namespace Flow.DependencyResolver.Internal.Graph;

internal sealed class Graph<TKey>(int startingCount, IEqualityComparer<TKey> comparer) where TKey : notnull
{
    public readonly Dictionary<TKey, List<TKey>> Forward = new(startingCount, comparer);
    public readonly Dictionary<TKey, List<TKey>> Reverse = new(startingCount, comparer);
}