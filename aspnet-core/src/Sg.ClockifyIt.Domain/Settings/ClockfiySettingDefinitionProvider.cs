using Volo.Abp.Settings;

namespace Sg.ClockifyIt.Settings;

public class ClockifyItSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(ClockifyItSettings.MySetting1));
    }
}
