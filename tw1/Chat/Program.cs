namespace Chat;

using System.Net;
using Utils;

/// <summary>
/// Internal class Program.
/// </summary>
internal class Program
{
    /// <summary>Public static Task main.</summary>
    /// <param name="args">Command line arguments.</param>
    /// <returns>Main task.</returns>
    public static async Task Main(string[] args)
    {
        if (args.Length < 1 || args.Length > 2)
        {
            Console.WriteLine("Usage: \"chat <port>\" to start a server on given port");
            Console.WriteLine(
                "Usage: \"chat <address> <port>\" to connect to a server on given address and port"
            );
            return;
        }

        if (!int.TryParse(args[args.Length - 1], out var port))
        {
            Console.WriteLine("Incorrect port given!");
            return;
        }

        Client client;

        if (args.Length == 1)
        {
            client = await MakeServerAndWaitForClient(port);
        }
        else
        {
            if (!IPAddress.TryParse(args[0], out var address))
            {
                Console.WriteLine("Incorrect address given!");
                return;
            }

            client = await MakeClientAndWaitForConnection(address, port);
        }

        var listeningTask = client.Listen(
            new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true }
        );

        while (client.Connected)
        {
            var consoleReadingTask = Task.Run(() => Console.ReadLine());
            if (Task.WaitAny(new Task[] { listeningTask, consoleReadingTask }) == 0)
            {
                break;
            }

            var message = Option<string>.From(await consoleReadingTask);
            message.Map(client.SendMessage);
            if (message is Option<string>.Some some && some.value == "exit")
            {
                break;
            }
        }
    }

    private static async Task<Client> MakeServerAndWaitForClient(int port)
    {
        var server = new Server(port);
        Console.WriteLine($"Start listening on localhost:{port}");
        Console.WriteLine($"Waiting for client to connect");
        var client = await server.WaitForClient();
        Console.WriteLine($"Client connected! You can sending messages");
        return client;
    }

    private static async Task<Client> MakeClientAndWaitForConnection(IPAddress address, int port)
    {
        var clientTask = Client.New(address, port);
        Console.WriteLine("Connecting to server!");
        var client = await clientTask;
        Console.WriteLine("Connected");
        return client;
    }
}
