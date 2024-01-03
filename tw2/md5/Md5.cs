namespace Md5;

/// <summary>
/// Static class providing methods for hashing directories using md5 hash function.
/// </summary>
public static class Md5Hasher
{
    /// <summary>
    /// Hashes directory synchronously.
    /// </summary>
    /// <param name="directory"> Directory to hash.</param>
    /// <returns> Hash of directory.</returns>
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

        return System
            .Security
            .Cryptography
            .MD5
            .HashData(stream.GetBuffer().Take((int)stream.Position).ToArray());
    }

    /// <summary>
    /// Hashes directory asynchronously.
    /// </summary>
    /// <param name="directory"> Directory to hash.</param>
    /// <returns> Hash of directory.</returns>
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

        return System
            .Security
            .Cryptography
            .MD5
            .HashData(stream.GetBuffer().Take((int)stream.Position).ToArray());
    }

    private static async Task<byte[]> HashFileAsync(FileInfo file) =>
        await System.Security.Cryptography.MD5.HashDataAsync(file.OpenRead());

    private static byte[] HashFile(FileInfo file) =>
        System.Security.Cryptography.MD5.HashData(file.OpenRead());
}
