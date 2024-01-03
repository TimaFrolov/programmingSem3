namespace Chat;

using System.Net.Sockets;
using Utils;

/// <summary>
/// A server that listens for a single client.
/// </summary>
public class Server
{
    private TcpListener socket;

    private Option<TcpClient> client;

    /// <summary>
    /// Initializes a new instance of the <see cref="Server"/> class.
    /// </summary>
    /// <param name="port">The port to listen on.</param>
    public Server(int port)
    {
        this.socket = new TcpListener(System.Net.IPAddress.Any, port);
        this.socket.Start();
        this.client = Option<TcpClient>.None;
    }

    /// <summary>
    /// Waits for a client to connect.
    /// </summary>
    /// <returns>The connected client.</returns>
    public async Task<Client> WaitForClient()
    {
        if (this.client.IsNone())
        {
            this.client = await this.socket.AcceptTcpClientAsync();
        }

        return new Client(this.client.Unwrap());
    }
}
