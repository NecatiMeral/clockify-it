using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Sg.ClockifyIt.Integrations.Dtos;
using Sg.ClockifyIt.Sync.Users;
using Sg.ClockifyIt.Workspaces;

namespace Sg.ClockifyIt.Integrations
{
    public class IntegrationContext
    {
        public IServiceProvider ServiceProvider { get; }
        public IConfiguration Configuration { get; }
        public UserInfo User { get; }
        public WorkspaceConfiguration Workspace { get; }
        public IReadOnlyList<TimeEntryDto> TimeEntries { get; }

        public IntegrationContext(IServiceProvider serviceProvider, IConfiguration configuration, UserInfo user, WorkspaceConfiguration workspace, IReadOnlyList<TimeEntryDto> timeEntries)
        {
            ServiceProvider = serviceProvider;
            Configuration = configuration;
            User = user;
            Workspace = workspace;
            TimeEntries = timeEntries;
        }
    }
}
