namespace MyNUnitWeb.Controllers;

using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MyNUnitWeb.Data;

[ApiController]
[Route("[controller]")]
[DbContext(typeof(DatabaseContext))]
public class AssemblyController : ControllerBase
{
    private DatabaseContext context;

    public AssemblyController(DatabaseContext context)
    {
        this.context = context;
    }

    private async Task<File> FileOfForm(IFormFile formFile)
    {
        File file = new();
        file.Name = formFile.FileName;
        file.Content = new byte[formFile.Length];
        await formFile.CopyToAsync(new MemoryStream(file.Content));
        file.ContentSha256 = await SHA256.HashDataAsync(new MemoryStream(file.Content));
        return file;
    }

    [HttpPost]
    public async Task<IActionResult> Post()
    {
        var formFiles = Request.Form.Files;
        Console.WriteLine($"Files amount: {formFiles.ToArray().Length}");
        Assembly assembly = new();
        assembly.Files = (await Task.WhenAll(formFiles.Select(FileOfForm))).ToList();
        Console.WriteLine($"Files: {String.Join(", ", assembly.Files.Select(file => file.Name))}");
        await this.context.Assemblies.AddAsync(assembly);
        await this.context.SaveChangesAsync();
        return Ok(assembly);
    }

    [HttpGet]
    public IEnumerable<Assembly> Get() => this.context.Assemblies.Include(x => x.Files).ToList();

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var assembly = await this.context.Assemblies.Include(a => a.Files)
            .FirstAsync(a => a.Id == id);

        if (assembly == null)
        {
            return NotFound();
        }

        return Ok(assembly);
    }

    [HttpPost("{id}/files")]
    public async Task<IActionResult> AddFile(int id)
    {
        var formFiles = Request.Form.Files;
        var assembly = await this.context.Assemblies.Include(a => a.Files)
            .FirstAsync(a => a.Id == id);
        assembly.Files.AddRange((await Task.WhenAll(formFiles.Select(FileOfForm))).ToList());
        await this.context.SaveChangesAsync();
        return Ok(assembly);
    }

    [HttpPost("{id}/runTest")]
    public async Task<IActionResult> StartNewTestRun(int id)
    {
        var assembly = await this.context.Assemblies.Include(a => a.Files)
            .FirstAsync(a => a.Id == id);

        if (assembly == null)
        {
            return NotFound();
        }

        var testRun = new TestRun { Assembly = assembly };

        try
        {
            var fileContents = assembly
                .Files.Select(file => file.Content)
                .Select(System.Reflection.Assembly.Load);

            testRun.TestResults = (
                await Task.WhenAll(
                    fileContents.Select(MyNUnit.TestRunner.RunAssemblyTests).ToArray()
                )
            )
                .SelectMany(results => results)
                .Select(result => new TestResult(result))
                .ToList();
        }
        catch (BadImageFormatException exception)
        {
            return BadRequest($"Loaded assembly files have bad format: {exception.Message}");
        }

        this.context.TestRuns.Add(testRun);

        await this.context.SaveChangesAsync();

        return Ok(testRun);
    }
}
