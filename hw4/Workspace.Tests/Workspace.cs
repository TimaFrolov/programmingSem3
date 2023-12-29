using System.Net;

namespace SimpleFTP.Tests;

public class WorkspaceTest
{
    static ((IPAddress, int), string[])[] workspaces =
    {
        ((IPAddress.Loopback, 3100), new string[] { }),
        ((IPAddress.Parse("134.56.120.15"), 3100), new string[] { "-a", "134.56.120.15" }),
        ((IPAddress.Parse("134.56.120.15"), 3100), new string[] { "--address", "134.56.120.15" }),
        ((IPAddress.Loopback, 5151), new string[] { "-p", "5151" }),
        ((IPAddress.Loopback, 5151), new string[] { "--port", "5151" }),
        (
            (IPAddress.Parse("134.56.120.15"), 5151),
            new string[] { "-a", "134.56.120.15", "-p", "5151" }
        ),
    };

    [TestCaseSource(nameof(workspaces))]
    public void TestWorkspace(((IPAddress, int) expected, string[] args) testCase)
    {
        var workspace = new Workspace(testCase.args);
        Assert.That(workspace.ServerAddress, Is.EqualTo(testCase.expected.Item1));
        Assert.That(workspace.ServerPort, Is.EqualTo(testCase.expected.Item2));
    }
}

