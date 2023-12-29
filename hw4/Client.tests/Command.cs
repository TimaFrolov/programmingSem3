namespace SimpleFTP.Client.Tests;

public class CommandTests
{
    static (Command, string)[] commands =
    {
        (new Command.List("dir1"), "list dir1"),
        (new Command.Get("file1"), "get file1"),
        (new Command.Help(), "help"),
        (new Command.Quit(), "quit"),
        (new Command.Quit(), "q"),
    };

    [TestCaseSource(nameof(commands))]
    public void TestParse((Command command, string expected) testCase)
    {
        var command = Command.TryParse(testCase.expected);
        Assert.That(command, Is.TypeOf<Utils.Option<Command>.Some>());
        var cmd = command.Unwrap();
        Assert.That(cmd, Is.EqualTo(testCase.command));
    }
}

