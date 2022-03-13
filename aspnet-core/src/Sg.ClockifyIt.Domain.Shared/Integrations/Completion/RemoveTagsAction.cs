using System.Linq;
using Sg.ClockifyIt.Integrations.Dtos;

namespace Sg.ClockifyIt.Integrations.Completion
{
    public class RemoveTagsAction : ICompletionAction
    {
        protected string[] TagsToRemove { get; }

        public RemoveTagsAction(string[] tagsToRemove)
        {
            TagsToRemove = tagsToRemove;
        }

        public void Complete(TimeEntryDto dto)
        {
            dto.Tags.RemoveAll(x => TagsToRemove.Contains(x.Name));
        }
    }
}
