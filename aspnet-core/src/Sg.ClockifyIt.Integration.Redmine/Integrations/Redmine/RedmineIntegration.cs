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
using Sg.ClockifyIt.Integrations.Completion;
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

                var issue = (await GetIssuesAsync(manager, new[] { issueId })).FirstOrDefault();

                foreach (var entry in timeEntries)
                {
                    try
                    {
                        await CreateTimeBookingAsync(manager, issue, entry);

                        result.MarkAsProcessed(entry.Id);
                    }
                    catch (Exception ex)
                    {
                        result.MarkAsFailed(entry.Id, ex);
                    }
                }
            }

            result.ApplyResultByConvention(context);

            return result;
        }

        protected TimeEntryToIssueMap ExtractIssueMap(IntegrationContext context, RedmineIntegrationOptions options)
        {
            var issueMap = new TimeEntryToIssueMap();
            foreach (var timeEntry in context.TimeEntries)
            {
                if (!timeEntry.Billable)
                {
                    continue;
                }

                var entryTagNames = timeEntry.GetTagNames();
                if (options.Tags.Any() && !options.Tags.Any(x => entryTagNames.Contains(x)))
                {
                    Logger.LogDebug("Skipping time entry `{TimeEntryId}` because of tag mismatch.", timeEntry.Id);
                    continue;
                }

                var redmineIssueIds = TaskExtractor.ExtractTaskFromDescription(timeEntry.Description, options.IssueIdExpression);
                if (!redmineIssueIds.Any())
                {
                    Logger.LogDebug("Skipping time entry `{TimeEntryId}` because it doesn't reference an redmine issue.", timeEntry.Id);
                    continue;
                }

                if (redmineIssueIds.Count > 1)
                {
                    Logger.LogDebug("Skipping time entry `{TimeEntryId}` because it references multiple redmine issues, which isn't supported.", timeEntry.Id);
                    continue;
                }

                issueMap.Map(timeEntry.Id, redmineIssueIds);
            }
            return issueMap;
        }

        protected virtual RedmineManager GetRedmineManager(RedmineIntegrationOptions options)
        {
            return new RedmineManager(options.Host, options.ApiKey, verifyServerCert: options.VerifyServerCert);
        }

        protected virtual async Task<List<Issue>> GetIssuesAsync(RedmineManager manager, string[] ids)
        {
            return await manager.GetObjectsAsync<Issue>(new NameValueCollection
            {
                { RedmineKeys.ISSUE_ID, string.Join(",", ids) }
            });
        }

        protected virtual async Task CreateTimeBookingAsync(RedmineManager manager, Issue issue, TimeEntryDto timeEntry)
        {
            var hours = XmlConvert.ToTimeSpan(timeEntry.TimeInterval.Duration);
            var booking = new TimeEntry
            {
                Issue = IdentifiableName.Create<IdentifiableName>(issue.Id),
                Hours = Convert.ToDecimal(hours.TotalHours),
                SpentOn = timeEntry.TimeInterval.Start.Value.Date,
                Comments = timeEntry.Description,
                Activity = IdentifiableName.Create<IdentifiableName>(9)
            };

            await manager.CreateObjectAsync(booking);
        }
    }
}
