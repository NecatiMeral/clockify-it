using System.Linq;
using Sg.ClockifyIt.Integrations.Dtos;

namespace Sg.ClockifyIt.Integrations.Completion
{
    public class AddTagsAction : ICompletionAction
    {
        protected string[] TagsToAdd { get; }
        protected string WorkspaceId { get; }

        public AddTagsAction(string[] tagsToAdd, string workspaceId)
        {
            TagsToAdd = tagsToAdd;
            WorkspaceId = workspaceId;
        }

        public void Complete(TimeEntryDto dto)
        {
            var currentTags = dto.Tags.Select(x => x.Name);
            var missingTags = TagsToAdd.Except(currentTags).ToArray();

            dto.Tags.AddRange(missingTags.Select(x => new TagDto
            {
                Id = string.Empty,
                Name = x,
                WorkspaceId = WorkspaceId
            }));
        }
    }
}
