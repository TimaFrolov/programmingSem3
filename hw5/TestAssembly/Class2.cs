namespace TestAssembly;

using System.Collections.Concurrent;

public class Class2 { 
    public static volatile ConcurrentQueue<TestEvent> events = new();
    Guid id;

    public Class2()
    {
        this.id = Guid.NewGuid();
        events.Enqueue(new InstanceCreated(this.id));
    }

    [@Test]
    public void Test()
    {
        events.Enqueue(new MethodCalled(this.id, nameof(Test)));
    }
}
