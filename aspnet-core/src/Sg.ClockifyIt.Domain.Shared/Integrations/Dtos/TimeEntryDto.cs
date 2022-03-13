using System.Collections.Generic;
using System.Linq;

namespace Sg.ClockifyIt.Integrations.Dtos
{
    public class TimeEntryDto
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public List<TagDto> Tags { get; set; }

        //public UserDto User { get; set; }

        public bool Billable { get; set; }

        public TaskDto Task { get; set; }

        //public ProjectDtoImpl Project { get; set; }

        public TimeIntervalDto TimeInterval { get; set; }

        public string WorkspaceId { get; set; }

        //public HourlyRateDto HourlyRate { get; set; }

        public string UserId { get; set; }

        public string ProjectId { get; set; }

        public bool IsLocked { get; set; }

        public TimeEntryDto()
        {
            Tags = new List<TagDto>();
        }

        public virtual string[] GetTagNames()
        {
            return Tags.Select(x => x.Name).ToArray();
        }
    }
}
