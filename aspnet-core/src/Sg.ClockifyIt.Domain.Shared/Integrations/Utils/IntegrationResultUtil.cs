using System.Linq;
using Microsoft.Extensions.Configuration;
using Sg.ClockifyIt.Integrations.Completion;

namespace Sg.ClockifyIt.Integrations.Utils
{
    public static class IntegrationResultUtil
    {
        public static void ApplyResultByConvention(this IntegrationResult result, IntegrationContext integrationContext, CommonIntegrationOptions options = null)
        {
            options ??= integrationContext.Configuration.Get<CommonIntegrationOptions>();

            result.AddAction(new RemoveBillableAction());
            if (options.Tags.Any())
            {
                result.AddAction(new RemoveTagsAction(options.Tags.ToArray()));
            }
            if (options.ProcessedTags.Any())
            {
                result.AddAction(new AddTagsAction(options.ProcessedTags.ToArray(), integrationContext.WorkspaceId));
            }
            if (options.ErrorTags.Any())
            {
                result.AddAction(new AddTagsAction(options.ErrorTags.ToArray(), integrationContext.WorkspaceId));
            }
        }
    }
}
