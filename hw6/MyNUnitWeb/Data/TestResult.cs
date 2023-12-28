namespace MyNUnitWeb.Data;

public class TestResult
{
    public int Id { get; set; }

    public string className { get; set; }
    public string methodName { get; set; }
    public string? reason { get; set; }
    public string? exception { get; set; }

    public TestResult(MyNUnit.TestResult testResult)
    {
        this.className = testResult.className;
        this.methodName = testResult.methodName;
        if (testResult is MyNUnit.TestResult.Error error)
        {
            this.exception = error.exception.Message;
        }
        else if (testResult is MyNUnit.TestResult.Ignored ignored)
        {
            this.reason = ignored.reason;
        }
    }

    private TestResult()
    {
        this.className = string.Empty;
        this.methodName = string.Empty;
    }
}
