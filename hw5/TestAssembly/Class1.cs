namespace TestAssembly;

using System.Collections.Concurrent;

public class Class1
{
    public static volatile ConcurrentQueue<TestEvent> events = new();
    Guid id;

    public Class1()
    {
        this.id = Guid.NewGuid();
        events.Enqueue(new InstanceCreated(this.id));
    }

    [BeforeClass]
    public static void StaticMethod1()
    {
        events.Enqueue(new StaticMethodCalled(nameof(StaticMethod1)));
    }

    [AfterClass]
    public static void StaticMethod2()
    {
        events.Enqueue(new StaticMethodCalled(nameof(StaticMethod2)));
    }

    [Before]
    public void Method1()
    {
        events.Enqueue(new MethodCalled(this.id, nameof(Method1)));
    }

    [After]
    public void Method2()
    {
        events.Enqueue(new MethodCalled(this.id, nameof(Method2)));
    }

    [Test]
    public void Test()
    {
        events.Enqueue(new MethodCalled(this.id, nameof(Test)));
    }

    [Test(Expected = true)]
    public void TestExpected()
    {
        events.Enqueue(new MethodCalled(this.id, nameof(TestExpected)));
        throw new ArgumentException();
    }

    [Test]
    public void TestFails()
    {
        events.Enqueue(new MethodCalled(this.id, nameof(TestFails)));
        throw new ArgumentException();
    }

    [Test(Expected = true)]
    public void TestExpectedFails()
    {
        events.Enqueue(new MethodCalled(this.id, nameof(TestExpectedFails)));
    }

    [Test(Ignore = "Testing ignore functionality")]
    public void TestIgnore()
    {
        events.Enqueue(new MethodCalled(this.id, nameof(TestIgnore)));
    }
}
