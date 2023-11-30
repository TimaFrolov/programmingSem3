namespace Md5;

internal class Program
{
    public static int Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine(
                "Pass directory path as only argument to application to get directory hash"
            );
            return 1;
        }

        var stopwatch = new System.Diagnostics.Stopwatch();

        try
        {
            stopwatch.Start();
            byte[] hash = Md5Hasher.HashDirectory(new DirectoryInfo(args[0]));
            stopwatch.Stop();
            Console.WriteLine(
                $"Synchrounous algorithm result: {string.Join("", hash.Select(x => $"{x:x2}"))}. "
                    + $"Elapsed time: {stopwatch.ElapsedMilliseconds} ms"
            );

            stopwatch.Restart();
            byte[] asyncHash = Md5Hasher.HashDirectoryAsync(new DirectoryInfo(args[0])).Result;
            stopwatch.Stop();
            Console.WriteLine(
                $"Asynchrounous algorithm result: {string.Join("", asyncHash.Select(x => $"{x:x2}"))}. "
                    + $"Elapsed time: {stopwatch.ElapsedMilliseconds} ms"
            );
        }
        catch (SystemException exception)
        {
            Console.WriteLine(exception.Message);
        }

        return 0;
    }
}
