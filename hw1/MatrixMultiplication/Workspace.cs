namespace MatrixMultiplication;

/// <summary>Workspace for matrix multiplication.</summary>
internal class Workspace
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Workspace"/> class from console arguments.
    /// </summary>
    /// <param name="args">Console arguments.</param>
    public Workspace(string[] args)
    {
        this.Mode = ModeType.Default;
        var files = new string[3];

        int filesAmount = 0;
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-m":
                    i++;
                    switch (args[i])
                    {
                        case "multithreading":
                            this.Mode = ModeType.Multithreading;
                            break;
                        case "transposition":
                            this.Mode = ModeType.Transposition;
                            break;
                        default:
                            throw new ArgumentException("Invalid mode");
                    }

                    this.Mode = ModeType.Transposition;
                    break;
                case "-h":
                case "--help":
                    this.Mode = ModeType.Help;
                    break;
                default:
                    if (filesAmount < 3)
                    {
                        files[filesAmount] = args[i];
                    }

                    filesAmount++;
                    break;
            }
        }

        if (this.Mode == ModeType.Help)
        {
            this.InputFile1 = string.Empty;
            this.InputFile2 = string.Empty;
            this.OutputFile = string.Empty;
            return;
        }

        if (filesAmount != 3)
        {
            throw new ArgumentException($"Invalid amount of files: {filesAmount}.");
        }

        this.InputFile1 = files[0];
        this.InputFile2 = files[1];
        this.OutputFile = files[2];
    }

    /// <summary>
    /// Possible modes of matrix multiplier program.
    /// </summary>
    public enum ModeType
    {
        /// <summary>
        /// Default mode.
        /// Multiply matrix from <see cref="InputFile1"/> by matrix from <see cref="InputFile2"/>
        /// and write result to <see cref="OutputFile"/>.
        /// </summary>
        Default,

        /// <summary>
        /// Default mode.
        /// Multiply matrix from <see cref="InputFile1"/> by matrix from <see cref="InputFile2"/>
        /// using transposition for optimization and write result to <see cref="OutputFile"/>.
        /// </summary>
        Transposition,

        /// <summary>
        /// Default mode.
        /// Multiply matrix from <see cref="InputFile1"/> by matrix from <see cref="InputFile2"/>
        /// using transposition and multithreading for optimization and write result to <see cref="OutputFile"/>.
        /// </summary>
        Multithreading,

        /// <summary>
        /// Show help message.
        /// </summary>
        Help,
    }

    /// <summary>
    /// Gets selected mode of matrix multiplier program.
    /// </summary>
    public ModeType Mode { get; private set; }

    /// <summary>
    /// Gets first input file path.
    /// </summary>
    public string InputFile1 { get; private set; }

    /// <summary>
    /// Gets second input file path.
    /// </summary>
    public string InputFile2 { get; private set; }

    /// <summary>
    /// Gets output file path.
    /// </summary>
    public string OutputFile { get; private set; }
}
