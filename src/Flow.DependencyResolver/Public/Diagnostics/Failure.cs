

namespace Flow.DependencyResolver.Diagnostics;

/*public class Failure<TKey> where TKey : notnull
{
    public readonly TKey? Key;
    public readonly IFailureReason Reason;
    public bool IsGlobal => Key is null;

    internal Failure(Option<TKey> key, IFailureReason reason) => (Key, Reason) = (default, reason);

    public static Failure<TKey> ForKey(TKey key, IFailureReason reason) => new(key, reason);
    public static Failure<TKey> Global(IFailureReason reason) => new(Option<TKey>.None, reason);

    public override string ToString()
    {
        return Key.Match(key => $"{key}: {Reason}", () => Reason.ToString() ?? string.Empty);
    }   
}*/

public class Failure : IFailure
{
    public IFailureReason Reason { get; init; }

    internal Failure(IFailureReason reason) => Reason = reason;

    public override string ToString() => Reason.ToString();
}

public class KeyedFailure<TKey> : IFailure where TKey : notnull
{
    public readonly TKey Key;
    public IFailureReason Reason { get; init; }

    internal KeyedFailure(TKey key, IFailureReason reason) => (Key, Reason) = (key, reason);

    public override string ToString() => $"{Key}: {Reason}";
}

public interface IFailure
{
    public IFailureReason Reason { get; init; }
}