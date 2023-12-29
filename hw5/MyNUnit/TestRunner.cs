namespace MyNUnit;

using System.Reflection;
using Utils;

/// <summary>
/// Represents the result of a test.
/// </summary>
/// <param name="className">The name of the class.</param>
/// <param name="methodName">The name of the method.</param>
public record TestResult(string className, string methodName)
{
    /// <summary>
    /// Represents a successful test.
    /// </summary>
    /// <inheritdoc cref="TestResult"/>
    /// <param name="elapsed">The time that test took to run.</param>
    public record Ok(string className, string methodName, TimeSpan elapsed) : TestResult(className, methodName);

    /// <summary>
    /// Represents a failed test.
    /// </summary>
    /// <param name="className"><inheritdoc cref="TestResult"/></param>
    /// <param name="methodName"><inheritdoc cref="TestResult"/></param>
    /// <param name="exception">The exception that caused the test to fail.</param>
    /// <param name="elapsed">The time that test took to run.</param>
    public record Error(string className, string methodName, Exception exception, TimeSpan elapsed)
        : TestResult(className, methodName);

    /// <summary>
    /// Represents a failed test.
    /// </summary>
    /// <param name="className"><inheritdoc cref="TestResult"/></param>
    /// <param name="methodName"><inheritdoc cref="TestResult"/></param>
    /// <param name="reason">The exception that caused the test to fail.</param>
    public record Ignored(string className, string methodName, string reason)
        : TestResult(className, methodName);

    /// <summary>
    /// Creates a new <see cref="TestResult"/> instance. <see cref="Ok"/> is created if
    /// <paramref name="exception"/> is <see cref="Option{Exception}.None"/>, <see cref="Error"/> is created
    /// otherwise.
    /// </summary>
    /// <param name="className"><inheritdoc cref="TestResult"/></param>
    /// <param name="methodName"><inheritdoc cref="TestResult"/></param>
    /// <param name="exception">The exception that caused the test to fail.</param>
    /// <param name="elapsed">The time that test took to run.</param>
    /// <returns>A new <see cref="TestResult"/> instance.</returns>
    public static TestResult From(
        string className,
        string methodName,
        Option<Exception> exception,
        TimeSpan elapsed
    ) =>
        exception.IsSome()
            ? new Error(className, methodName, exception.Unwrap(), elapsed)
            : new Ok(className, methodName, elapsed);
}

/// <summary>
/// Tests runner. Runs tests from the specified assembly.
/// </summary>
public static class TestRunner
{
    /// <summary>
    /// Runs tests from the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly to run tests from.</param>
    /// <returns>The results of the tests.</returns>
    public static async Task<IEnumerable<TestResult>> RunAssemblyTests(Assembly assembly)
    {
        var tasks = assembly.ExportedTypes.Select(TestRunner.RunClassTests);
        return (await Task.WhenAll(tasks)).SelectMany(x => x);
    }

    private static async Task<IEnumerable<TestResult>> RunClassTests(Type type)
    {
        foreach (var method in type.GetMethods())
        {
            if (method.IsStatic && method.GetCustomAttribute<BeforeClass>() != null)
            {
                method.Invoke(null, new object[0]);
            }
        }

        var runTestIfMethodHasTestAttribute = (MethodInfo method) =>
            Option<Test>
                .From(method.GetCustomAttribute<Test>())
                .Map(attr => Task.Run(() => RunTest(type, method, attr)));

        var tasks = type.GetMethods()
            .Select(runTestIfMethodHasTestAttribute)
            .Where(x => x.IsSome())
            .Select(x => x.Unwrap());

        var ret = await Task.WhenAll(tasks);

        foreach (var method in type.GetMethods())
        {
            if (method.IsStatic && method.GetCustomAttribute<AfterClass>() != null)
            {
                method.Invoke(null, new object[0]);
            }
        }

        return ret;
    }

    private static TestResult RunTest(Type type, MethodInfo method, Test testAttr)
    {
        if (testAttr.Ignore != null)
        {
            return new TestResult.Ignored(type.Name, method.Name, testAttr.Ignore);
        }

        var instance = Activator.CreateInstance(type);

        foreach (var before in type.GetMethods())
        {
            if (!before.IsStatic && before.GetCustomAttribute<Before>() != null)
            {
                before.Invoke(instance, new object[0]);
            }
        }

        TestResult ret;
        var watch = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            method.Invoke(instance, null);
            watch.Stop();
            ret = TestResult.From(
                type.Name,
                method.Name,
                testAttr.Expected
                    ? new Exception("Expected exception was not thrown")
                    : Option<Exception>.None,
                watch.Elapsed
            );
        }
        catch (Exception exception)
        {
            watch.Stop();
            ret = TestResult.From(
                type.Name,
                method.Name,
                testAttr.Expected ? Option<Exception>.None : exception.InnerException!,
                watch.Elapsed
            );
        }

        foreach (var after in type.GetMethods())
        {
            if (!after.IsStatic && after.GetCustomAttribute<After>() != null)
            {
                after.Invoke(instance, new object[0]);
            }
        }

        return ret;
    }
}
