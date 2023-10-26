namespace SimpleFTP;

using System.Net;

/// <summary>
/// The workspace of the SimpleFTP server or client.
/// </summary>
public class Workspace
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Workspace"/> class from command line arguments.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    public Workspace(string[] args)
    {
        this.ServerAddress = IPAddress.Loopback;
        this.ServerPort = 3100;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-a":
                case "--address":
                    this.ServerAddress = IPAddress.Parse(args[++i]);
                    break;
                case "-p":
                case "--port":
                    this.ServerPort = int.Parse(args[++i]);
                    break;
            }
        }
    }

    /// <summary>
    /// Gets the address of the server.
    /// </summary>
    public IPAddress ServerAddress { get; }

    /// <summary>
    /// Gets the port of the server.
    /// </summary>
    public int ServerPort { get; }
}
