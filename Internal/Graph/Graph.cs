namespace Flow.DependencyResolver.Internal.Graph;

internal sealed class Graph<TKey> where TKey : notnull
{
    public readonly Dictionary<TKey, List<TKey>> Forward = [];
    public readonly Dictionary<TKey, List<TKey>> Reverse = [];
}