namespace SimpleFTP;

using System.Net.Sockets;
using Utils;

/// <summary> Internal class Program.</summary>
internal class Program
{
    /// <summary> The entry point of the program. </summary>
    /// <param name="args">The command line arguments.</param>
    /// <returns>Main task.</returns>
    public static async Task Main(string[] args)
    {
        var workspace = new Workspace(args);
        var server = new TcpListener(workspace.ServerAddress, workspace.ServerPort);
        server.Start();
        while (true)
        {
#pragma warning disable CS4014 // Never ment to await HandleClient
            HandleClient(await server.AcceptTcpClientAsync());
        }
    }

    private static async Task HandleClient(TcpClient client)
    {
        using var stream = client.GetStream();
        using var reader = new StreamReader(stream);
        using var writer = new StreamWriter(stream) { AutoFlush = true };

        while (client.Connected)
        {
            var response = await Option<string>
                .From(await reader.ReadLineAsync())
                .AndThen(Request.TryFrom)
                .MapOr(GetAnswer, Response.UnknownRequest);

            Try<System.IO.IOException>.Call(() => writer.WriteLineAsync(response.ToString()));
        }
    }

    private static async Task<Response> GetAnswer(Request request)
    {
        if (request is Request.List list)
        {
            var files = Try<System.IO.IOException>.Call(
                () => Directory.GetFiles(list.path).Select(x => (x, false))
            );
            if (!files.IsOk())
            {
                return new Response.Error(files.UnwrapErr().Message);
            }

            var directories = Try<System.IO.IOException>.Call(
                () => Directory.GetDirectories(list.path).Select(x => (x, true))
            );
            if (!directories.IsOk())
            {
                return new Response.Error(directories.UnwrapErr().Message);
            }

            return new Response.List(files.Unwrap().Concat(directories.Unwrap()).ToArray());
        }
        else if (request is Request.Get get)
        {
            var body = await Try<System.IO.IOException>.CallAsync(
                async () => await File.ReadAllTextAsync(get.path)
            );
            return body.Map(x => (Response)new Response.Get(x))
                .UnwrapOrElse(x => new Response.Error(x.Message));
        }
        else
        {
            return Response.UnknownRequest;
        }
    }
}
