using System.Linq;
using Sg.ClockifyIt.Integrations.Dtos;

namespace Sg.ClockifyIt.Integrations.Completion
{
    public class AddTagsAction : ICompletionAction
    {
        protected string[] TagsToAdd { get; }
        protected string WorkspaceId { get; }
        protected string[] TimeEntryIds { get; }

        public AddTagsAction(string[] tagsToAdd, string workspaceId, string[] timeEntryIds = null)
        {
            TagsToAdd = tagsToAdd;
            WorkspaceId = workspaceId;
            TimeEntryIds = timeEntryIds;
        }

        public void Complete(TimeEntryDto dto)
        {
            if (TimeEntryIds == null || TimeEntryIds.Contains(dto.Id))
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
}
