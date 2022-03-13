using Sg.ClockifyIt.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace Sg.ClockifyIt.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(ClockifyItEntityFrameworkCoreModule),
    typeof(ClockifyItApplicationContractsModule)
    )]
public class ClockifyItDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
    }
}
