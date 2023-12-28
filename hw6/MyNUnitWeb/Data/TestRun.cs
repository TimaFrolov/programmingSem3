namespace MyNUnitWeb.Data;

public class TestRun
{
    public int Id { get; set; }

    public Assembly Assembly { get; set; }

    public List<TestResult> TestResults { get; set; } = new();
}
