using Sg.ClockifyIt.Integrations;
using Sg.ClockifyIt.Integrations.RedmineOverDevOps;
using Volo.Abp.Modularity;

namespace Sg.ClockifyIt
{
    [DependsOn(
        typeof(ClockifyItDomainSharedModule)
    )]
    public class ClockifyItRedmineOverDevOpsIntegrationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<IntegrationOptions>(options =>
            {
                options.Integrations[RedmineOverDevOpsIntegration.Name] = typeof(RedmineOverDevOpsIntegration);
            });
        }
    }
}
