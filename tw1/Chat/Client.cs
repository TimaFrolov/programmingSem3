namespace Chat;

using System.Net.Sockets;
using Utils;

/// <summary>
/// A client that can send and receive messages.
/// </summary>
public class Client
{
    private TcpClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class.
    /// </summary>
    /// <param name="client">The client to wrap.</param>
    public Client(TcpClient client) => this.client = client;

    /// <summary>
    /// Gets a value indicating whether the client is connected.
    /// </summary>
    public bool Connected => this.client.Connected;

    /// <summary>
    /// Creates a new client connected to a server with given address and port.
    /// </summary>
    /// <param name="address">The address of the server.</param>
    /// <param name="port">The port of the server.</param>
    /// <returns>The new client.</returns>
    public static async Task<Client> New(System.Net.IPAddress address, int port)
    {
        var client = new TcpClient();
        await client.ConnectAsync(address, port);
        return new Client(client);
    }

    /// <summary>
    /// Listens for messages from the server. Writes them to given writer.
    /// </summary>
    /// <param name="writer">The writer to write messages to.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Listen(StreamWriter writer)
    {
        using var stream = this.client.GetStream();
        using var reader = new StreamReader(stream);

        while (this.client.Connected)
        {
            Option<string>.From(await reader.ReadLineAsync()).Map(writer.WriteLineAsync);
        }
    }

    /// <summary>
    /// Sends a message to the server.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <returns>Whether the message was sent successfully.</returns>
    public async Task<bool> SendMessage(string message) =>
        (
            await Try<System.IO.IOException>.CallAsync(
                () =>
                    new StreamWriter(this.client.GetStream()) { AutoFlush = true }.WriteAsync(
                        message + "\n"
                    )
            )
        ).IsNone();
}
