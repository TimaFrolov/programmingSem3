namespace Chat;

using System.Net.Sockets;
using Utils;

public class Server
{
    TcpListener socket;

    Option<TcpClient> client;

    public Server(int port)
    {
        this.socket = new TcpListener(System.Net.IPAddress.Any, port);
        this.socket.Start();
        this.client = Option<TcpClient>.None;
    }

    public async Task<Client> WaitForClient() =>
        new Client(await this.socket.AcceptTcpClientAsync());
}
