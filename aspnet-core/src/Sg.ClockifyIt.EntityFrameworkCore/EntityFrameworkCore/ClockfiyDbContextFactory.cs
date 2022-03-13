using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Sg.ClockifyIt.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class ClockifyItDbContextFactory : IDesignTimeDbContextFactory<ClockifyItDbContext>
{
    public ClockifyItDbContext CreateDbContext(string[] args)
    {
        ClockifyItEfCoreEntityExtensionMappings.Configure();

        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<ClockifyItDbContext>()
            .UseSqlite(configuration.GetConnectionString("Default"));

        return new ClockifyItDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Sg.ClockifyIt.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
