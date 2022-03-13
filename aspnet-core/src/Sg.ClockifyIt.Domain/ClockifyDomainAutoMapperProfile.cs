using AutoMapper;
using Clockify.Net.Models.Tags;
using Clockify.Net.Models.Tasks;
using Clockify.Net.Models.TimeEntries;

namespace Sg.ClockifyIt
{
    public class ClockifyItDomainAutoMapperProfile : Profile
    {
        public ClockifyItDomainAutoMapperProfile()
        {
            CreateMap<HydratedTimeEntryDtoImpl, Integrations.Dtos.TimeEntryDto>();
            CreateMap<TagDto, Integrations.Dtos.TagDto>();
            CreateMap<TaskDto, Integrations.Dtos.TaskDto>();
            CreateMap<TimeIntervalDto, Integrations.Dtos.TimeIntervalDto>();
            CreateMap<TaskStatus, Integrations.Dtos.TaskStatus>();
        }
    }
}
