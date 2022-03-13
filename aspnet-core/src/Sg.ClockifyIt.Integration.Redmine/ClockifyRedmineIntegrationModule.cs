using Sg.ClockifyIt.Integrations;
using Sg.ClockifyIt.Integrations.Redmine;
using Volo.Abp.Modularity;

namespace Sg.ClockifyIt
{
    [DependsOn(
        typeof(ClockifyItDomainSharedModule)
    )]
    public class ClockifyItRedmineIntegrationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<IntegrationOptions>(options =>
            {
                options.Integrations[RedmineIntegration.Name] = typeof(RedmineIntegration);
            });
        }
    }
}
