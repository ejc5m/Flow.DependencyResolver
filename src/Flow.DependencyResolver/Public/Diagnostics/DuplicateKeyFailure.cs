namespace Flow.DependencyResolver.Diagnostics;

public record DuplicateKeyFailure<TKey>(TKey Duplicate) : IFailureReason
{
    public override string ToString() => $"Duplicate key '{Duplicate}'.";
}