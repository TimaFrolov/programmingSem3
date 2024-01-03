public record TestEvent
{
    public record StaticMethodCalled(string MethodName) : TestEvent;

    public record InstanceCreated(Guid id) : TestEvent;

    public record MethodCalled(Guid instance, string MethodName) : TestEvent;
}
