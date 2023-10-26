namespace SimpleFTP.Client;

using System.Net.Sockets;
using System.Text.RegularExpressions;
using Utils;

/// <summary> Internal class Program.</summary>
internal class Program
{
    private const string HelpMessage =
        "Use one of the following commands:\n"
        + "list <path> - make list request to server\n"
        + "get <path> - make get request to server, save file to current directory\n"
        + "help - get this message\n"
        + "quit - exit from program";

    /// <summary> The entry point of the program. </summary>
    /// <param name="args">The command line arguments.</param>
    /// <returns>Main task.</returns>
    public static async Task Main(string[] args)
    {
        var workspace = new Workspace(args);
        var client = new TcpClient();
        await client.ConnectAsync(workspace.ServerAddress, workspace.ServerPort);
        using var stream = client.GetStream();
        using var reader = new StreamReader(stream);
        using var writer = new StreamWriter(stream) { AutoFlush = true };

        while (true)
        {
            Console.Write("> ");
            switch (Option<string>.From(Console.ReadLine()).Map(Command.TryParse))
            {
                case Option<Option<Command>>.Some(Option<Command>.Some(Command.List(var path))):
                {
                    await writer.WriteLineAsync(new Request.List(path).ToString());
                    var response = Option<string>
                        .From(await reader.ReadLineAsync())
                        .AndThen(Response.List.TryParse);
                    if (response is Option<Response.List>.Some(var list))
                    {
                        Console.WriteLine(
                            string.Join(
                                '\n',
                                list.list.Select(
                                    file => $"{file.name} - {(file.isDir ? "directory" : "file")}"
                                )
                            )
                        );
                    }
                    else
                    {
                        Console.WriteLine("Server gave invalid response");
                    }

                    break;
                }

                case Option<Option<Command>>.Some(Option<Command>.Some(Command.Get(var path))):
                {
                    await writer.WriteLineAsync(new Request.Get(path).ToString());
                    if (await Response.Get.TryParseAsync(reader) is Option<Response.Get>.Some(var get))
                    {
                        await File.WriteAllTextAsync(
                            new Regex(@"([^/]+/)*(.+)").Match(path).Groups[2].Value,
                            get.data
                        );
                        Console.WriteLine("File saved");
                    }
                    else
                    {
                        Console.WriteLine("Server gave invalid response");
                    }

                    break;
                }

                case Option<Option<Command>>.Some(Option<Command>.Some(Command.Help)):
                case Option<Option<Command>>.Some(Option<Command>._None):
                {
                    Console.WriteLine(HelpMessage);
                    break;
                }

                case Option<Option<Command>>.Some(Option<Command>.Some(Command.Quit)):
                    return;
            }
        }
    }
}
