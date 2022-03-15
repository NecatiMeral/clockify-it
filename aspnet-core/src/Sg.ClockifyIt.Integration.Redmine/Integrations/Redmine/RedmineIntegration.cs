using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Redmine.Net.Api;
using Redmine.Net.Api.Async;
using Redmine.Net.Api.Types;
using Sg.ClockifyIt.Integrations.Dtos;
using Sg.ClockifyIt.Integrations.Utils;
using Volo.Abp.DependencyInjection;

namespace Sg.ClockifyIt.Integrations.Redmine
{
    public class RedmineIntegration : IClockifyItIntegration, ITransientDependency
    {
        public const string Name = "Redmine@0";

        public ILogger<RedmineIntegration> Logger { get; set; }

        public RedmineIntegration()
        {
            Logger = NullLogger<RedmineIntegration>.Instance;
        }

        public async Task<IntegrationResult> ProcessAsync(IntegrationContext context)
        {
            var options = context.Configuration.Get<RedmineIntegrationOptions>();

            var issueMap = ExtractIssueMap(context, options);
            var issueIds = issueMap.GetDistinctReferencedIssueIds();
            var manager = GetRedmineManager(options);

            var result = new IntegrationResult();

            foreach (var issueId in issueIds)
            {
                var timeEntryIds = issueMap.GetTimeEntriesByIssueId(issueId);
                var timeEntries = context.TimeEntries.Where(x => timeEntryIds.Contains(x.Id)).ToArray();

                try
                {
                    var issue = await GetIssueAsync(manager, issueId);

                    foreach (var entry in timeEntries)
                    {
                        try
                        {
                            var activityId = GetActivityId(options.Activities, entry.Tags);

                            await CreateTimeBookingAsync(manager, issue, entry, activityId);

                            result.MarkAsProcessed(entry.Id);
                        }
                        catch (Exception individualEntryException)
                        {
                            // We failed at processing a specific entry, set it in a failed state
                            // and attempt to process other entries referencing the current issue.
                            result.MarkAsFailed(entry.Id, individualEntryException);
                        }
                    }
                }
                catch (Exception workItemException)
                {
                    // We haven't even come to the point where we process the indivudual items; 
                    // set all entries in a failed state.
                    foreach (var timeEntry in timeEntries)
                    {
                        result.MarkAsFailed(timeEntry.Id, workItemException);
                    }
                }
            }

            result.ApplyResultByConvention(context);

            return result;
        }

        protected TimeEntryMap<int> ExtractIssueMap(IntegrationContext context, RedmineIntegrationOptions options)
        {
            var issueMap = new TimeEntryMap<int>();
            foreach (var timeEntry in context.TimeEntries)
            {
                if (!timeEntry.Billable)
                {
                    continue;
                }

                var entryTagNames = timeEntry.GetTagNames();
                if (options.Tags.Any() && !options.Tags.All(x => x.MatchesTags(entryTagNames)))
                {
                    Logger.LogDebug("Skipping time entry `{TimeEntryId}` because of tag mismatch.", timeEntry.Id);
                    continue;
                }

                var workItemIds = TaskExtractor.ExtractTaskFromDescription(timeEntry.Description, options.IssueIdExpression);
                if (!workItemIds.Any())
                {
                    Logger.LogDebug("Skipping time entry `{TimeEntryId}` because it doesn't reference an redmine issue.", timeEntry.Id);
                    continue;
                }

                if (workItemIds.Count > 1)
                {
                    Logger.LogDebug("Skipping time entry `{TimeEntryId}` because it references multiple redmine issues, which isn't supported.", timeEntry.Id);
                    continue;
                }

                issueMap.Map(timeEntry.Id, workItemIds);
            }
            return issueMap;
        }

        protected virtual RedmineManager GetRedmineManager(RedmineIntegrationOptions options)
        {
            return new RedmineManager(options.Host, options.ApiKey, verifyServerCert: options.VerifyServerCert);
        }

        protected virtual async Task<Issue> GetIssueAsync(RedmineManager manager, int id)
        {
            return await manager.GetObjectAsync<Issue>(id.ToString(), new NameValueCollection());
        }

        protected virtual int GetActivityId(ActivityTagDictionary activities, List<TagDto> tags)
        {
            foreach (var tag in tags)
            {
                if (activities.TryGetValue(tag.Name, out var activityId))
                {
                    return activityId;
                }
            }

            return activities.Default;
        }

        protected virtual async Task CreateTimeBookingAsync(RedmineManager manager, Issue issue, TimeEntryDto timeEntry, int activityId)
        {
            var hours = XmlConvert.ToTimeSpan(timeEntry.TimeInterval.Duration);
            var booking = new TimeEntry
            {
                Issue = IdentifiableName.Create<IdentifiableName>(issue.Id),
                Hours = Convert.ToDecimal(hours.TotalHours),
                SpentOn = timeEntry.TimeInterval.Start.Value.Date,
                Comments = timeEntry.Description
            };

            if (activityId > 0)
            {
                booking.Activity = IdentifiableName.Create<IdentifiableName>(activityId);
            }
            else
            {
                Logger.LogInformation("Attempting to create time entry without activity since no configured activity matched the current time entry.");
            }

            await manager.CreateObjectAsync(booking);
        }
    }
}
