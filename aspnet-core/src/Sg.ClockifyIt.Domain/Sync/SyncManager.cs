using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clockify.Net;
using Clockify.Net.Models.TimeEntries;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Sg.ClockifyIt.Clockify.Tags;
using Sg.ClockifyIt.ClockifyIt;
using Sg.ClockifyIt.Integrations;
using Sg.ClockifyIt.Integrations.Dtos;
using Sg.ClockifyIt.Sync.Users;
using Sg.ClockifyIt.Workspaces;
using Volo.Abp;
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
        protected IClockifyTagProvider ClockifyTagProvider { get; }
        protected IntegrationManager IntegrationManager { get; }

        public ILogger<SyncManager> Logger { get; set; }

        public SyncManager(IOptions<WorkspaceOptions> options,
            IClockifyItClientFactory clockifyItClientFactory,
            IUserInfoProvider userInfoProvider,
            IObjectMapper objectMapper,
            IClockifyTagProvider clockifyTagProvider,
            IntegrationManager integrationManager)
        {
            Options = options.Value;
            ClockifyItClientFactory = clockifyItClientFactory;
            UserInfoProvider = userInfoProvider;
            ObjectMapper = objectMapper;
            ClockifyTagProvider = clockifyTagProvider;
            IntegrationManager = integrationManager;

            Logger = NullLogger<SyncManager>.Instance;
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
            AssertWorkspaceIsValid(configuration, workspaceName);

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

            var resultMap = await IntegrationManager.RunIntegrationsAsync(workspaceId, configuration, user, timeEntryDtos);

            var processedTimeEntries = timeEntryDtos.Where(x => resultMap.ContainsKey(x.Id) && resultMap[x.Id].Succeed).ToList();

            foreach (var processedItem in processedTimeEntries)
            {
                foreach (var action in resultMap.CompletionActions)
                {
                    action.Complete(processedItem);
                }
            }

            await PostProcessTimeEntriesAsync(client, user, processedTimeEntries, workspaceId);
            await MarkTimeEntriesAsBilledAsync(client, user, processedTimeEntries, workspaceId);
        }

        protected virtual async Task PostProcessTimeEntriesAsync(ClockifyClient client, UserInfo user, List<TimeEntryDto> timeEntries, string workspaceId)
        {
            var tagsWithoutId = timeEntries.SelectMany(x => x.Tags)
                .Where(x => x.Id.IsNullOrEmpty())
                .ToArray();

            if (!tagsWithoutId.Any())
            {
                return;
            }

            var distinctTagNames = tagsWithoutId.Select(x => x.Name).Distinct().ToArray();

            foreach (var missingTagName in distinctTagNames)
            {
                var tag = await ClockifyTagProvider.GetOrCreateTagByName(client, workspaceId, missingTagName);

                var currentTagsWithMissingId = tagsWithoutId.Where(x => x.Name == missingTagName).ToList();
                currentTagsWithMissingId.ForEach(x =>
                {
                    x.Id = tag.Id;
                });
            }
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

        protected virtual void AssertWorkspaceIsValid(WorkspaceConfiguration workspace, string workspaceName)
        {
            if (workspace.ApiKey.IsNullOrWhiteSpace())
            {
                throw new BusinessException(ClockifyItDomainErrorCodes.InvalidConfiguration, logLevel: LogLevel.Error)
                    .WithData("WorkspaceName", workspaceName)
                    .WithData("ConfigurationKey", nameof(WorkspaceConfiguration.ApiKey));
            }
        }
    }
}
