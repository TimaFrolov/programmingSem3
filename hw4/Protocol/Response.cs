namespace SimpleFTP;

using Utils;

/// <summary>
/// The response sent by the server to the client.
/// </summary>
public abstract record Response
{
    /// <summary>
    /// The response to list the files and directories of a directory.
    /// </summary>
    /// <param name="list">The list of files and directories.</param>
    public sealed record List((string name, bool isDir)[] list) : Response
    {
        /// <summary>
        /// Tries to parse a response from a string.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <returns>The parsed response or <see cref="Option{List}.None"/> on error.</returns>
        public static Option<List> TryParse(string str) =>
            Option<(string name, bool isDir)>
                .Collect(
                    string.Join(string.Empty, str.ToArray().SkipWhile(x => x != ' ').Skip(1))
                        .Split(' ')
                        .Select((x, index) => (x, index))
                        .GroupBy(x => x.index / 2)
                        .Select(
                            entry =>
                                (
                                    name: entry.First().x,
                                    isDir: Try<System.InvalidOperationException>
                                        .Call(() => entry.Skip(1).First().x)
                                        .TryUnwrap()
                                        .AndThen(TryParseBool)
                                )
                        )
                        .Select(
                            ((string name, Option<bool> isDir) entry) =>
                                (Option<(string name, bool isDir)>)(
                                    entry.isDir.IsSome()
                                        ? (entry.name, entry.isDir.Unwrap())
                                        : Option<(string name, bool isDir)>.None
                                )
                        )
                        .ToArray()
                )
                .Map(x => new List(x.ToArray()));

        /// <inheritdoc cref="Response"/>
        public override string ToString() =>
            $"{this.list.Length} {string.Join(' ', this.list.Select(file => $"{file.name} {(file.isDir ? "true" : "false")}"))}";

        private static Option<bool> TryParseBool(string str) =>
            str == "true"
                ? true
                : str == "false"
                    ? false
                    : Option<bool>.None;
    }

    /// <summary>
    /// The response to get the content of a file.
    /// </summary>
    public sealed record Get(string data) : Response
    {
        /// <summary>
        /// Tries to parse a response from a stream.
        /// </summary>
        /// <param name="reader">The stream to parse.</param>
        /// <returns>The parsed response or <see cref="Option{Get}.None"/> on error.</returns>
        public static async Task<Option<Get>> TryParseAsync(StreamReader reader)
        {
            var length = await ParseIntAsync(reader);
            var buf = new char[length];

            var read = await reader.ReadBlockAsync(buf, 0, length);

            return read == length ? new Get(string.Join(string.Empty, buf)) : Option<Get>.None;
        }

        private static async Task<int> ParseIntAsync(StreamReader reader)
        {
            var buf = new char[1];
            var answer = 0;
            while (true)
            {
                await reader.ReadBlockAsync(buf, 0, 1);
                if (buf[0] < '0' || buf[0] > '9')
                {
                    break;
                }

                answer = (answer * 10) + buf[0] - '0';
            }

            return answer;
        }

        /// <inheritdoc cref="Response"/>
        public override string ToString() =>
            $"{this.data.Length} {string.Join(string.Empty, this.data.Select(x => (char)x))}";
    }

    /// <summary>
    /// The error response.
    /// </summary>
    public sealed record Error(string message) : Response
    {
        /// <inheritdoc cref="Response"/>
        public override string ToString() => $"Error: {this.message}";
    }

    /// <summary>
    /// The unknown request response. This is singleton class. Use <see cref="UnknownRequest"/>.
    /// </summary>
    public sealed record _UnknownRequest() : Response
    {
        /// <inheritdoc cref="Response"/>
        public override string ToString() => "Unknown request";
    }

    /// <summary>
    /// Gets the instance of <see cref="_UnknownRequest"/>.
    /// </summary>
    public static _UnknownRequest UnknownRequest { get; } = new _UnknownRequest();

    /// <summary>
    /// String representation of the response.
    /// </summary>
    /// <returns>The string representation of the request.</returns>
    public abstract override string ToString();

    private Response() { }
}
