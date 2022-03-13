using Sg.ClockifyIt.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Sg.ClockifyIt.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class ClockifyItController : AbpControllerBase
{
    protected ClockifyItController()
    {
        LocalizationResource = typeof(ClockifyItResource);
    }
}
