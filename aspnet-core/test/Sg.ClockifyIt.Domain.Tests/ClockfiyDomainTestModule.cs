using Sg.ClockifyIt.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Sg.ClockifyIt;

[DependsOn(
    typeof(ClockifyItEntityFrameworkCoreTestModule)
    )]
public class ClockifyItDomainTestModule : AbpModule
{

}
