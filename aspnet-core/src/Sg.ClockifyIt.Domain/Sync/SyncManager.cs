using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clockify.Net;
using Clockify.Net.Models.TimeEntries;
using Microsoft.Extensions.Options;
using Sg.ClockifyIt.ClockifyIt;
using Sg.ClockifyIt.Integrations;
using Sg.ClockifyIt.Integrations.Dtos;
using Sg.ClockifyIt.Sync.Users;
using Sg.ClockifyIt.Workspaces;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;

namespace Sg.ClockifyIt.Sync
{
    public class SyncManager : ITransientDependency
    {
        protected WorkspaceOptions Options { get; }
        protected IClockifyItClientFactory ClockifyItClientFactory { get; }
        protected IUserInfoProvider UserInfoProvider { get; }
        protected IObjectMapper ObjectMapper { get; }
        protected IntegrationManager IntegrationManager { get; }

        public SyncManager(IOptions<WorkspaceOptions> options,
            IClockifyItClientFactory ClockifyItClientFactory,
            IUserInfoProvider userInfoProvider,
            IObjectMapper objectMapper,
            IntegrationManager integrationManager)
        {
            Options = options.Value;
            ClockifyItClientFactory = ClockifyItClientFactory;
            UserInfoProvider = userInfoProvider;
            ObjectMapper = objectMapper;
            IntegrationManager = integrationManager;
        }

        public virtual async Task SyncAsync()
        {
            var workspaces = Options.Workspaces.Keys;

            foreach (var workspace in workspaces)
            {
                await SyncWorkspaceAsync(workspace);
            }
        }

        protected virtual async Task SyncWorkspaceAsync(string workspaceName)
        {
            var configuration = Options.GetWorkspace(workspaceName);
            if (!IsWorkspaceValid(configuration))
            {
                return;
            }

            var client = ClockifyItClientFactory.CreateClient(configuration);
            var user = await UserInfoProvider.GetCurrentUserInfoAsync(client, workspaceName);

            foreach (var workspaceId in user.WorkspaceIds)
            {
                await SyncWorkspaceAsync(workspaceName, client, user, configuration, workspaceId);
            }
        }

        protected virtual async Task SyncWorkspaceAsync(string workspaceName, ClockifyClient client, UserInfo user, WorkspaceConfiguration configuration, string workspaceId)
        {
            var fetchEnd = DateTimeOffset.Now.Add(-configuration.Delay);
            var fetchStart = fetchEnd - configuration.FetchRange;

            var timeEntries = await client.FindAllHydratedTimeEntriesForUserAsync(workspaceId, user.Id, start: fetchStart, end: fetchEnd);
            if (timeEntries.Data.Count == 0)
            {
                return;
            }

            var timeEntryDtos = ObjectMapper.Map<List<HydratedTimeEntryDtoImpl>, List<TimeEntryDto>>(timeEntries.Data);

            var resultMap = await IntegrationManager.RunIntegrationsAsync(configuration, user, timeEntryDtos);

            var processedTimeEntries = timeEntryDtos.Where(x => resultMap.ContainsKey(x.Id) && resultMap[x.Id]).ToList();
            foreach (var processedItem in processedTimeEntries)
            {
                foreach (var action in resultMap.CompletionActions)
                {
                    action.Complete(processedItem);
                }
            }

            await MarkTimeEntriesAsBilledAsync(client, user, processedTimeEntries, workspaceId);
        }

        protected virtual async Task MarkTimeEntriesAsBilledAsync(ClockifyClient client, UserInfo user, List<TimeEntryDto> timeEntries, string workspaceId)
        {
            foreach (var timeEntry in timeEntries)
            {
                await client.UpdateTimeEntryAsync(workspaceId,
                    timeEntry.Id,
                    new UpdateTimeEntryRequest
                    {
                        Description = timeEntry.Description,
                        Start = timeEntry.TimeInterval.Start,
                        End = timeEntry.TimeInterval.End,
                        ProjectId = timeEntry.ProjectId,
                        TaskId = timeEntry.Task?.Id,
                        Billable = timeEntry.Billable,
                        TagIds = timeEntry.Tags.Select(x => x.Id).ToList()
                    }
                );
            }
        }

        protected virtual bool IsWorkspaceValid(WorkspaceConfiguration workspace)
        {
            return !workspace.ApiKey.IsNullOrWhiteSpace();
        }
    }
}
