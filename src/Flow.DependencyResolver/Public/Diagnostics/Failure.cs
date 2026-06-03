namespace Flow.DependencyResolver.Diagnostics;

public class Failure : IFailure
{
    public IFailureReason Reason { get; init; }

    internal Failure(IFailureReason reason) => Reason = reason;

    public override string ToString() => Reason.ToString();
}