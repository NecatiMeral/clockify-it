using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;

namespace Sg.ClockifyIt;

[DependsOn(
    typeof(ClockifyItDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpObjectExtendingModule)
)]
public class ClockifyItApplicationContractsModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        ClockifyItDtoExtensions.Configure();
    }
}
