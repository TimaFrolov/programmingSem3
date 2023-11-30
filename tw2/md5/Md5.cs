namespace Md5;

public static class Md5Hasher
{
    private static async Task<byte[]> HashFileAsync(FileInfo file) =>
        await System.Security.Cryptography.MD5.HashDataAsync(file.OpenRead());

    private static byte[] HashFile(FileInfo file) =>
        System.Security.Cryptography.MD5.HashData(file.OpenRead());

    public static byte[] HashDirectory(DirectoryInfo directory)
    {
        MemoryStream stream = new();
        stream.Write(System.Text.Encoding.UTF8.GetBytes(directory.Name));

        var subdirectories = directory.GetDirectories();
        Array.Sort(subdirectories.Select(x => x.Name).ToArray(), subdirectories);
        var files = directory.GetFiles();
        Array.Sort(files.Select(file => file.Name).ToArray(), files);

        var hashingTasks = subdirectories.Select(HashDirectory).Concat(files.Select(HashFile));
        foreach (var hash in hashingTasks)
        {
            stream.Write(hash);
        }

        stream.Flush();
        return System.Security.Cryptography.MD5.HashData(stream.GetBuffer());
    }

    public static async Task<byte[]> HashDirectoryAsync(DirectoryInfo directory)
    {
        MemoryStream stream = new();
        stream.Write(System.Text.Encoding.UTF8.GetBytes(directory.Name));

        var subdirectories = directory.GetDirectories();
        Array.Sort(subdirectories.Select(x => x.Name).ToArray(), subdirectories);
        var files = directory.GetFiles();
        Array.Sort(files.Select(file => file.Name).ToArray(), files);

        var hashingTasks = subdirectories
            .Select(HashDirectoryAsync)
            .Concat(files.Select(HashFileAsync));
        foreach (var hash in await Task.WhenAll(hashingTasks))
        {
            stream.Write(hash);
        }

        return System.Security.Cryptography.MD5.HashData(stream.GetBuffer());
    }
}
