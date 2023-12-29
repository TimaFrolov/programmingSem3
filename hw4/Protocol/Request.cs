namespace SimpleFTP;

using Utils;

/// <summary>
/// The request sent by the client to the server.
/// </summary>
public abstract record Request
{
    /// <summary>
    /// The request to list the files and directories of a directory.
    /// </summary>
    public sealed record List(string path) : Request
    {
        /// <inheritdoc cref="Request"/>
        public override string ToString() => $"1 {this.path}";
    }

    /// <summary>
    /// The request to get the content of a file.
    /// </summary>
    public sealed record Get(string path) : Request
    {
        /// <inheritdoc cref="Request"/>
        public override string ToString() => $"2 {this.path}";
    }

    /// <summary>
    /// Tries to parse a request from a string.
    /// </summary>
    /// <param name="str">The string to parse.</param>
    /// <returns>The parsed request or <see cref="Option{Request}.None"/> on error.</returns>
    public static Option<Request> TryFrom(string str) =>
        str.Length > 2
            ? str[0] == '1'
                ? new List(str.Substring(2))
                : str[0] == '2'
                    ? new Get(str.Substring(2))
                    : Option<Request>.None
            : Option<Request>.None;

    /// <summary>
    /// String representation of the request.
    /// </summary>
    /// <returns>The string representation of the request.</returns>
    public abstract override string ToString();

    private Request() { }
}
