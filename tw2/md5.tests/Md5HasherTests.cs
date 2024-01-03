namespace Md5.tests;

using System.Security.Cryptography;
using System.Text;

public record FileSystemObject
{
    public record Dir(string name, FileSystemObject[] objects) : FileSystemObject { }

    public record File(string path, byte[] content) : FileSystemObject { }

    public async Task CreateIn(DirectoryInfo directory)
    {
        if (this is Dir(var dirPath, var objects))
        {
            var dir = directory.CreateSubdirectory(dirPath);
            await Task.WhenAll(objects.Select(obj => obj.CreateIn(dir)));
            return;
        }
        if (this is File(var filePath, var content))
        {
            await System
                .IO
                .File
                .WriteAllBytesAsync(Path.Combine(directory.FullName, filePath), content);
            return;
        }
    }

    private FileSystemObject() { }
}

public class Tests
{
    static (FileSystemObject.Dir dir, byte[] hash)[] testCases = new[]
    {
        (
            new FileSystemObject.Dir("b", new FileSystemObject[] { }),
            MD5.HashData(Encoding.UTF8.GetBytes("b"))
        ),
        (
            new FileSystemObject.Dir(
                "a",
                new FileSystemObject[] { new FileSystemObject.File("t", new byte[] { 97, 98, 99 }) }
            ),
            MD5.HashData(
                Encoding
                    .UTF8
                    .GetBytes("a")
                    .Concat(MD5.HashData(new byte[] { 97, 98, 99 }))
                    .ToArray()
            )
        ),
        (
            new FileSystemObject.Dir(
                "c",
                new FileSystemObject[]
                {
                    new FileSystemObject.File("t2", new byte[] { 97, 98, 99 }),
                    new FileSystemObject.File("t1", new byte[] { 100, 101, 102 })
                }
            ),
            MD5.HashData(
                Encoding
                    .UTF8
                    .GetBytes("c")
                    .Concat(MD5.HashData(new byte[] { 100, 101, 102 }))
                    .Concat(MD5.HashData(new byte[] { 97, 98, 99 }))
                    .ToArray()
            )
        ),
    };

    [TestCaseSource(nameof(testCases))]
    public async Task Test((FileSystemObject.Dir dir, byte[] hash) testCase)
    {
        var directory = Directory.CreateTempSubdirectory();
        await testCase.dir.CreateIn(directory);
        var testDir = new DirectoryInfo(Path.Combine(directory.FullName, testCase.dir.name));
        Assert.That(Md5Hasher.HashDirectory(testDir), Is.EqualTo(testCase.hash));
        Assert.That(await Md5Hasher.HashDirectoryAsync(testDir), Is.EqualTo(testCase.hash));
    }
}

