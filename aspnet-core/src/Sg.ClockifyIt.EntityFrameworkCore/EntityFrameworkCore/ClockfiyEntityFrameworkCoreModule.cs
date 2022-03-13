using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Modularity;

namespace Sg.ClockifyIt.EntityFrameworkCore;

[DependsOn(
    typeof(ClockifyItDomainModule),
    typeof(AbpEntityFrameworkCoreSqliteModule)
    )]
public class ClockifyItEntityFrameworkCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        ClockifyItEfCoreEntityExtensionMappings.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<ClockifyItDbContext>(options =>
        {
            /* Remove "includeAllEntities: true" to create
             * default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: true);
        });

        Configure<AbpDbContextOptions>(options =>
        {
            /* The main point to change your DBMS.
             * See also ClockifyItMigrationsDbContextFactory for EF Core tooling. */
            options.UseSqlite();
        });
    }
}
