using System.Net.Sockets;

namespace Chat.tests;

public class ClientTests
{
#pragma warning disable CS8618 // initialising in Setup, not in constructor
    TcpListener listener;
    TcpClient server;
    Client client;
#pragma warning restore CS8618

    [SetUp]
    public void Setup()
    {
        if (this.listener == null)
        {
            this.listener = new TcpListener(System.Net.IPAddress.Any, 20000);
            this.listener.Start();
        }
        this.client = Client.New(System.Net.IPAddress.Loopback, 20000).Result;
        this.server = listener.AcceptTcpClient();
    }

    [Test]
    public void testSending()
    {
        Assert.True(this.client.Connected);
        this.client.SendMessage("test").Wait();
        Assert.That(new StreamReader(this.server.GetStream()).ReadLine(), Is.EqualTo("test"));
    }

    [Test]
    public void testListening()
    {
        Assert.True(this.client.Connected);
        var stream = new MemoryStream(5);
        var task = this.client.Listen(new StreamWriter(stream));
        new StreamWriter(stream) { AutoFlush = true }.Write("test\n");
        Assert.That(
            stream.GetBuffer(),
            Is.EqualTo(new byte[] { (byte)'t', (byte)'e', (byte)'s', (byte)'t', (byte)'\n' })
        );
    }
}
