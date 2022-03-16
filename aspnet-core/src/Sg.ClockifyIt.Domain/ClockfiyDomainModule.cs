using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sg.ClockifyIt.Localization;
using Sg.ClockifyIt.MultiTenancy;
using Sg.ClockifyIt.Workspaces;
using Volo.Abp;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Emailing;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
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
    typeof(AbpLocalizationModule),
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

        context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("Sq.ClockifyIt", typeof(ClockifyItResource));
        });

        Configure<WorkspaceOptions>(configuration.GetSection(ClockifyConfigurationConsts.ClockifyConfigurationSectionName));
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
