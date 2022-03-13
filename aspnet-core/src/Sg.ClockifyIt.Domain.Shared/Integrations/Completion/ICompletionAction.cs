using Sg.ClockifyIt.Integrations.Dtos;

namespace Sg.ClockifyIt.Integrations.Completion
{
    public interface ICompletionAction
    {
        void Complete(TimeEntryDto dto);
    }
}
