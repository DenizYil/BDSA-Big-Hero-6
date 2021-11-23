using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CoProject.Infrastructure;
[ExcludeFromCodeCoverage]
public class CoProjectContextFactory : IDesignTimeDbContextFactory<CoProjectContext>
{
    public CoProjectContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddUserSecrets<CoProjectContext>()
            .Build();

        var connectionString = configuration.GetConnectionString("CoProject");

        var optionsBuilder = new DbContextOptionsBuilder<CoProjectContext>()
            .UseSqlServer(connectionString);

        return new CoProjectContext(optionsBuilder.Options);
    }
}