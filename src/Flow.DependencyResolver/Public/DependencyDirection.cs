namespace Flow.DependencyResolver;

public enum DependencyDirection
{
    //Item must come after the specified key
    After,
    //Item must come before the specified key
    Before,
}