namespace MyNUnit;

/// <summary>
/// Attribute to mark a test method.
/// Method with this attribute will be run by test runner.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class Test : Attribute
{
    /// <summary>
    /// Gets or sets a value indicating whether test is expected to throw an exception.
    /// </summary>
    public bool Expected { get; set; }

    /// <summary>
    /// Gets or sets the reason why the test is ignored.
    /// </summary>
    public string? Ignore { get; set; }
}

/// <summary>
/// Attribute to mark a method to be run before each test method.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class Before : Attribute { }

/// <summary>
/// Attribute to mark a method to be run after each test method.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class After : Attribute { }

/// <summary>
/// Attribute to mark a method to be run before all test methods.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class BeforeClass : Attribute { }

/// <summary>
/// Attribute to mark a method to be run after all test methods.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class AfterClass : Attribute { }
