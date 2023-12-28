namespace MyNUnitWeb.Data;

using Microsoft.EntityFrameworkCore;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options) { }

    public DbSet<Assembly> Assemblies => Set<Assembly>();

    public DbSet<TestRun> TestRuns => Set<TestRun>();
}
