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
                result.AddAction(new RemoveTagsAction(options.Tags.SelectMany(x => x).ToArray()));
            }

            var succeedEntryIds = result.Where(x => x.Value.Succeed).Select(x => x.Key).ToArray();
            if (options.ProcessedTags.Any() && succeedEntryIds.Any())
            {
                result.AddAction(new AddTagsAction(options.ProcessedTags.ToArray(), integrationContext.WorkspaceId, succeedEntryIds));
            }

            var failedEntryIds = result.Where(x => !x.Value.Succeed).Select(x => x.Key).ToArray();
            if (options.ErrorTags.Any() && failedEntryIds.Any())
            {
                result.AddAction(new AddTagsAction(options.ErrorTags.ToArray(), integrationContext.WorkspaceId, failedEntryIds));
            }
        }
    }
}
