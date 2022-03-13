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
            ApiUrl = "https://api.clockify.me/api/v1";
            ExperimentalApiUrl = "https://api.clockify.me/api/";
            ReportsApiUrl = "https://reports.clockify.me/v1";
        }
    }
}
