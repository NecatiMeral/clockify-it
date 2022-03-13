using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Sg.ClockifyIt.Workspaces
{
    public class WorkspaceOptions
    {
        public WorkspaceConfigurationDictionary Workspaces { get; set; }

        public WorkspaceOptions()
        {
            Workspaces = new WorkspaceConfigurationDictionary();
        }

        public WorkspaceConfiguration GetWorkspace(string workspaceName = null)
        {
            if (workspaceName.IsNullOrWhiteSpace())
            {
                workspaceName = WorkspaceConfigurationDictionary.DefaultName;
            }

            return MergeWorkspaces(Workspaces.GetOrDefault(workspaceName));
        }

        protected WorkspaceConfiguration MergeWorkspaces(WorkspaceConfiguration configuration)
        {
            return new WorkspaceConfiguration
            {
                ApiKey = configuration?.ApiKey ?? Workspaces.Default.ApiKey,
                ApiUrl = configuration?.ApiUrl ?? Workspaces.Default.ApiUrl,
                ExperimentalApiUrl = configuration?.ExperimentalApiUrl ?? Workspaces.Default.ExperimentalApiUrl,
                ReportsApiUrl = configuration?.ReportsApiUrl ?? Workspaces.Default.ReportsApiUrl,
                Integrations = configuration.Integrations
                    .Union(Workspaces.Default.Integrations, new IntegrationNameComparer())
                    .ToArray(),
                Delay = configuration?.Delay ?? Workspaces.Default.Delay,
                FetchRange = configuration?.FetchRange ?? Workspaces.Default.FetchRange
            };
        }

        class IntegrationNameComparer : IEqualityComparer<WorkspaceIntegrationReference>
        {
            public bool Equals(WorkspaceIntegrationReference x, WorkspaceIntegrationReference y)
            {
                return x.Name == y.Name;
            }

            public int GetHashCode(WorkspaceIntegrationReference obj)
            {
                return obj.Name.GetHashCode();
            }
        }
    }
}
