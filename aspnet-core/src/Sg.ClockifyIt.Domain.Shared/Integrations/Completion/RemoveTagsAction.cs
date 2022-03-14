using System.Linq;
using Sg.ClockifyIt.Integrations.Dtos;

namespace Sg.ClockifyIt.Integrations.Completion
{
    public class RemoveTagsAction : ICompletionAction
    {
        protected string[] TagsToRemove { get; }
        protected string[] TimeEntryIds { get; }

        public RemoveTagsAction(string[] tagsToRemove, string[] timeEntryIds = null)
        {
            TagsToRemove = tagsToRemove;
            TimeEntryIds = timeEntryIds;
        }

        public void Complete(TimeEntryDto dto)
        {
            if (TimeEntryIds == null || TimeEntryIds.Contains(dto.Id))
            {
                dto.Tags.RemoveAll(x => TagsToRemove.Contains(x.Name));
            }
        }
    }
}
