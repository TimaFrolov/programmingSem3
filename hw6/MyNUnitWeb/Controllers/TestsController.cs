namespace MyNUnitWeb.Controllers;

using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MyNUnitWeb.Data;

[ApiController]
[Route("[controller]")]
[DbContext(typeof(DatabaseContext))]
public class TestsController : ControllerBase
{
    private DatabaseContext context;

    public TestsController(DatabaseContext context)
    {
        this.context = context;
    }

    [HttpGet]
    public IEnumerable<TestRun> Get() =>
        this.context.TestRuns.Include(x => x.TestResults).Include(x => x.Assembly).ToList();

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var test = await this.context.TestRuns.Include(a => a.TestResults)
            .FirstAsync(a => a.Id == id);

        if (test == null)
        {
            return NotFound();
        }

        return Ok(test);
    }
}
