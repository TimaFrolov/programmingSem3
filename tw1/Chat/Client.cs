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
            var message = Option<string>.From(await reader.ReadLineAsync());
            message.Map(writer.WriteLineAsync);
            if (message is Option<string>.Some some && some.value == "exit")
            {
                client.Close();
            }
        }
    }

    public async Task<bool> SendMessage(string message) =>
        (
            await Try<System.IO.IOException>.CallAsync(
                async () =>
                    await new StreamWriter(this.client.GetStream())
                    {
                        AutoFlush = true
                    }.WriteLineAsync(message)
            )
        ).IsNone();
}
