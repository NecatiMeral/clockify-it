using System;
using System.Collections.Generic;
using System.Text;
using Sg.ClockifyIt.Localization;
using Volo.Abp.Application.Services;

namespace Sg.ClockifyIt;

/* Inherit your application services from this class.
 */
public abstract class ClockifyItAppService : ApplicationService
{
    protected ClockifyItAppService()
    {
        LocalizationResource = typeof(ClockifyItResource);
    }
}
