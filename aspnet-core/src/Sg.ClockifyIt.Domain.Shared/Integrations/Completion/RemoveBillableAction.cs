using Sg.ClockifyIt.Integrations.Dtos;

namespace Sg.ClockifyIt.Integrations.Completion
{
    public class RemoveBillableAction : ICompletionAction
    {
        public void Complete(TimeEntryDto dto)
        {
            dto.Billable = false;
        }
    }
}
