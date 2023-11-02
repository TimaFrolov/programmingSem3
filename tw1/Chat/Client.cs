namespace Chat;

using System.Net.Sockets;
using Utils;

public class Client
{
    TcpClient client;

    public Client(TcpClient client) => this.client = client;

    public static async Task<Client> New(System.Net.IPAddress address, int port)
    {
        var client = new TcpClient();
        await client.ConnectAsync(address, port);
        return new Client(client);
    }

    public bool Connected => this.client.Connected;

    public async Task Listen(StreamWriter writer)
    {
        using var stream = this.client.GetStream();
        using var reader = new StreamReader(stream);

        while (this.client.Connected)
        {
            Option<string>.From(await reader.ReadLineAsync()).Map(writer.WriteLineAsync);
        }
    }

    public async Task<bool> SendMessage(string message) =>
        (
            await Try<System.IO.IOException>.CallAsync(async () =>
            {
                using var writer = new StreamWriter(this.client.GetStream()) { AutoFlush = true };
                await writer.WriteLineAsync(message);
            })
        ).IsNone();
}
