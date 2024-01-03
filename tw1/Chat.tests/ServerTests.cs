using System.Net.Sockets;

namespace Chat.tests;

public class ServerTests
{
    [Test]
    public void ServerReturnsClient()
    {
        var server = new Server(20001);
        var client = new TcpClient();
        client.Connect(System.Net.IPAddress.Loopback, 20001);
        var serverClient = server.WaitForClient().Result;
        serverClient.SendMessage("test").Wait();
        var arr = new byte[5];
        client.GetStream().Read(arr);
        Assert.That(
            arr,
            Is.EqualTo(new byte[] { (byte)'t', (byte)'e', (byte)'s', (byte)'t', (byte)'\n' })
        );
    }
}
