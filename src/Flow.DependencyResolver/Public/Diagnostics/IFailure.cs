namespace Flow.DependencyResolver.Diagnostics;

public interface IFailure
{
    public IFailureReason Reason { get; init; }
}