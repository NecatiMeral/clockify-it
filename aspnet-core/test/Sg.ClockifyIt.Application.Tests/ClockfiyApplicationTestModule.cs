using Volo.Abp.Modularity;

namespace Sg.ClockifyIt;

[DependsOn(
    typeof(ClockifyItApplicationModule),
    typeof(ClockifyItDomainTestModule)
    )]
public class ClockifyItApplicationTestModule : AbpModule
{

}
