namespace SimpleFTP.Client;

using System.Text.RegularExpressions;
using Utils;

/// <summary>
/// The command given by user.
/// </summary>
public record Command
{
    /// <summary>
    /// The command to list the files and directories of a directory.
    /// </summary>
    public sealed record List(string path) : Command;

    /// <summary>
    /// The command to get the content of a file.
    /// </summary>
    public sealed record Get(string path) : Command;

    /// <summary>
    /// The command to get help.
    /// </summary>
    public sealed record Help() : Command;

    /// <summary>
    /// The command to quit the program.
    /// </summary>
    public sealed record Quit() : Command;

    /// <summary>
    /// Tries to parse a command from a string.
    /// </summary>
    /// <param name="str">The string to parse.</param>
    /// <returns>The parsed command or <see cref="Option{Command}.None"/> on error.</returns>
    public static Option<Command> TryParse(string str)
    {
        var match = new Regex(@"^(\w{1,4})( (\S+))?$").Match(str);

        var command = match.Groups[1].Value;

        switch (command)
        {
            case "list":
                return new List(match.Groups[3].Value);
            case "get":
                return new Get(match.Groups[3].Value);
            case "help":
                return new Help();
            case "quit":
            case "q":
                return new Quit();
            default:
                return Option<Command>.None;
        }
    }

    private Command() { }
}
