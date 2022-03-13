using Clockify.Net;
using Sg.ClockifyIt.Workspaces;

namespace Sg.ClockifyIt.ClockifyIt
{
    public interface IClockifyItClientFactory
    {
        ClockifyClient CreateClient(WorkspaceConfiguration workspaceConfiguration);
    }
}
