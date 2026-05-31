using Flow.DependencyResolver.Internal.Graph;
using System;
using System.Collections.Generic;

namespace Flow.DependencyResolver;

public static class DependencyGraph
{
    public static DependencyGraphBuilder<TKey> Create<TKey>() where TKey : notnull => new();


    public class DependencyGraphBuilder<TKey> where TKey : notnull
    {
        internal readonly List<DependencyNode<TKey>> Nodes = [];

        private DependencyNodeBuilder<TKey>? _currentNode;

        public DependencyNodeBuilder<TKey> Add(TKey key)
        {
            _currentNode = new DependencyNodeBuilder<TKey>(key, this);
            return _currentNode;
        }

        internal DependencyResolutionResult<TKey> Resolve() => DependencyResolver.Resolve(Nodes);
    }

    public class DependencyNodeBuilder<TKey> where TKey : notnull
    {
        private readonly TKey Key;

        List<Dependency<TKey>> Node = [];

        private readonly DependencyGraphBuilder<TKey> _parent;

        internal DependencyNodeBuilder(TKey key, DependencyGraphBuilder<TKey> parent)
        {
            Key = key;
            _parent = parent;
        }

        public DependencyNodeBuilder<TKey> DependsOn(params TKey[] keys)
        {
            foreach (var key in keys)
                Node.Add(new Dependency<TKey>(key));
            return this;
        }

        public DependencyNodeBuilder<TKey> OptionallyDependsOn(params TKey[] keys)
        {
            foreach (var key in keys)
                Node.Add(new Dependency<TKey>(key).Optional());
            return this;
        }

        public DependencyNodeBuilder<TKey> Add(TKey key)
        {
            _parent.Nodes.Add(new(Key, Node));
            return _parent.Add(key);
        }

        public DependencyResolutionResult<TKey> Resolve()
        {
            _parent.Nodes.Add(new(Key, Node));
            return _parent.Resolve();
        }
    }
}

