using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sg.ClockifyIt.MultiTenancy;
using Sg.ClockifyIt.Workspaces;
using Volo.Abp;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Emailing;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;

namespace Sg.ClockifyIt;

[DependsOn(
    typeof(ClockifyItDomainSharedModule),
    typeof(AbpDddDomainModule),
    typeof(AbpEmailingModule),
    typeof(AbpCachingModule),
    typeof(AbpBackgroundWorkersModule),
    typeof(AbpAutoMapperModule)
)]
public class ClockifyItDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            //Add all mappings defined in the assembly of the MyModule class
            options.AddMaps<ClockifyItDomainModule>(validate: true);
        });

        var configuration = context.Services.GetConfiguration();
        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = MultiTenancyConsts.IsEnabled;
        });

#if DEBUG
        context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());
#endif

        Configure<WorkspaceOptions>(configuration);
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(() => OnApplicationInitializationAsync(context));
    }

    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        await context.AddBackgroundWorkerAsync<ClockifyItBackgroundWorker>();
    }
}
