using System;

namespace Sg.ClockifyIt.Workspaces
{
    public class WorkspaceConfiguration
    {
        public string ApiKey { get; set; }

        public string ApiUrl { get; set; }

        public string ExperimentalApiUrl { get; set; }

        public string ReportsApiUrl { get; set; }

        public TimeSpan Delay { get; set; }

        public TimeSpan FetchRange { get; set; }

        public WorkspaceIntegrationReference[] Integrations { get; set; }

        public WorkspaceConfiguration()
        {
            Integrations = Array.Empty<WorkspaceIntegrationReference>();
        }
    }
}
