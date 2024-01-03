using System.Reflection;

namespace MyNUnit.tests;

public class TestRunnerTests
{
    static Dictionary<
        (string type, string method),
        (Type result, string[] beforeClass, string[] before, string[] after, string[] afterClass)
    > tests =
        new()
        {
            {
                (nameof(TestAssembly.Class1), nameof(TestAssembly.Class1.Test)),
                (
                    typeof(TestResult.Ok),
                    new string[] { nameof(TestAssembly.Class1.StaticMethod1) },
                    new string[] { nameof(TestAssembly.Class1.Method1) },
                    new string[] { nameof(TestAssembly.Class1.Method2) },
                    new string[] { nameof(TestAssembly.Class1.StaticMethod2) }
                )
            },
            {
                (nameof(TestAssembly.Class1), nameof(TestAssembly.Class1.TestExpected)),
                (
                    typeof(TestResult.Ok),
                    new string[] { nameof(TestAssembly.Class1.StaticMethod1) },
                    new string[] { nameof(TestAssembly.Class1.Method1) },
                    new string[] { nameof(TestAssembly.Class1.Method2) },
                    new string[] { nameof(TestAssembly.Class1.StaticMethod2) }
                )
            },
            {
                (nameof(TestAssembly.Class1), nameof(TestAssembly.Class1.TestFails)),
                (
                    typeof(TestResult.Error),
                    new string[] { nameof(TestAssembly.Class1.StaticMethod1) },
                    new string[] { nameof(TestAssembly.Class1.Method1) },
                    new string[] { nameof(TestAssembly.Class1.Method2) },
                    new string[] { nameof(TestAssembly.Class1.StaticMethod2) }
                )
            },
            {
                (nameof(TestAssembly.Class1), nameof(TestAssembly.Class1.TestExpectedFails)),
                (
                    typeof(TestResult.Error),
                    new string[] { nameof(TestAssembly.Class1.StaticMethod1) },
                    new string[] { nameof(TestAssembly.Class1.Method1) },
                    new string[] { nameof(TestAssembly.Class1.Method2) },
                    new string[] { nameof(TestAssembly.Class1.StaticMethod2) }
                )
            },
            {
                (nameof(TestAssembly.Class1), nameof(TestAssembly.Class1.TestIgnore)),
                (
                    typeof(TestResult.Ignored),
                    new string[] { nameof(TestAssembly.Class1.StaticMethod1) },
                    new string[] { nameof(TestAssembly.Class1.Method1) },
                    new string[] { nameof(TestAssembly.Class1.Method2) },
                    new string[] { nameof(TestAssembly.Class1.StaticMethod2) }
                )
            },
            {
                (nameof(TestAssembly.Class2), nameof(TestAssembly.Class2.Test)),
                (
                    typeof(TestResult.Ok),
                    new string[] { },
                    new string[] { },
                    new string[] { },
                    new string[] { }
                )
            },
        };

    [NUnit.Framework.Test]
    public void Test()
    {
        TestAssembly.Class1.events.Clear();
        TestAssembly.Class2.events.Clear();
        var testResults = TestRunner
            .RunAssemblyTests(Assembly.GetAssembly(typeof(TestAssembly.Class1))!)
            .Result;
        foreach (var testResult in testResults)
        {
            try
            {
                Assert.IsInstanceOf(
                    tests[(testResult.className, testResult.methodName)].result,
                    testResult
                );
            }
            catch (KeyNotFoundException)
            {
                Assert.Fail($"Unexpected test: {testResult.className}.{testResult.methodName})");
            }
        }

        foreach (var test in tests)
        {
            var (type, method) = test.Key;
            var (result, beforeClass, before, after, afterClass) = test.Value;
            if (result == typeof(TestResult.Ignored))
            {
                continue;
            }
            TestEvent[] events;
            if (type == nameof(TestAssembly.Class1))
            {
                events = TestAssembly.Class1.events.ToArray();
            }
            else if (type == nameof(TestAssembly.Class2))
            {
                events = TestAssembly.Class2.events.ToArray();
            }
            else
            {
                Assert.Fail($"Unexpected class: {type}");
                return;
            }
            var instanceId = events
                .Where(x => x is TestEvent.MethodCalled(_, var m) && m == method)
                .Select(x => (x as TestEvent.MethodCalled)!.instance)
                .First();
            foreach (var beforeClassMethod in beforeClass)
            {
                Assert.IsTrue(
                    IsBefore(
                        events,
                        new TestEvent.StaticMethodCalled(beforeClassMethod),
                        new TestEvent.InstanceCreated(instanceId)
                    )
                );
            }
            foreach (var beforeMethod in before)
            {
                Assert.IsTrue(
                    IsBefore(
                        events,
                        new TestEvent.MethodCalled(instanceId, beforeMethod),
                        new TestEvent.MethodCalled(instanceId, method)
                    )
                );
            }
            foreach (var afterMethod in after)
            {
                Assert.IsTrue(
                    IsBefore(
                        events,
                        new TestEvent.MethodCalled(instanceId, method),
                        new TestEvent.MethodCalled(instanceId, afterMethod)
                    )
                );
            }
            foreach (var afterClassMethod in afterClass)
            {
                Assert.IsTrue(
                    IsBefore(
                        events,
                        new TestEvent.InstanceCreated(instanceId),
                        new TestEvent.StaticMethodCalled(afterClassMethod)
                    )
                );
            }
        }
    }

    private static bool IsBefore(IEnumerable<TestEvent> events, TestEvent first, TestEvent second)
    {
        int firstIndex = -1;
        int secondIndex = -1;
        for (int i = 0; i < events.Count(); i++)
        {
            if (events.ElementAt(i).Equals(first))
            {
                if (firstIndex != -1)
                {
                    Assert.Fail("Duplicate event");
                }
                firstIndex = i;
            }
            if (events.ElementAt(i).Equals(second))
            {
                if (secondIndex != -1)
                {
                    Assert.Fail("Duplicate event");
                }
                secondIndex = i;
            }
        }
        if (firstIndex == -1)
        {
            Assert.Fail("First event not found");
        }
        if (secondIndex == -1)
        {
            Assert.Fail("Second event not found");
        }
        return firstIndex < secondIndex;
    }
}
