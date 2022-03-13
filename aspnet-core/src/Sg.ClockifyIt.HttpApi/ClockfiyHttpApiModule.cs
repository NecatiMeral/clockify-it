using Localization.Resources.AbpUi;
using Sg.ClockifyIt.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace Sg.ClockifyIt;

[DependsOn(
    typeof(ClockifyItApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule)
    )]
public class ClockifyItHttpApiModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        ConfigureLocalization();
    }

    private void ConfigureLocalization()
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<ClockifyItResource>()
                .AddBaseTypes(
                    typeof(AbpUiResource)
                );
        });
    }
}
