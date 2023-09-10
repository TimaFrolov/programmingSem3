namespace MatrixMultiplication;

internal class Workspace
{
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
            throw new ArgumentException(
                "Usage: MatrixMultiplication [-m mode] <first file> <second file> <output file>"
            );
        }

        this.InputFile1 = files[0];
        this.InputFile2 = files[1];
        this.OutputFile = files[2];
    }

    public enum ModeType
    {
        Default,
        Transposition,
        Multithreading,
        Help,
    }

    public ModeType Mode { get; private set; }

    public string InputFile1 { get; private set; }

    public string InputFile2 { get; private set; }

    public string OutputFile { get; private set; }
}
