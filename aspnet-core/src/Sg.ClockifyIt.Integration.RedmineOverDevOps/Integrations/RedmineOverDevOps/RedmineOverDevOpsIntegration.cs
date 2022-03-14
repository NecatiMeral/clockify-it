using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Redmine.Net.Api;
using Redmine.Net.Api.Async;
using Redmine.Net.Api.Types;
using Sg.ClockifyIt.Integrations.Dtos;
using Sg.ClockifyIt.Integrations.Redmine;
using Sg.ClockifyIt.Integrations.Utils;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Sg.ClockifyIt.Integrations.RedmineOverDevOps
{
    public class RedmineOverDevOpsIntegration : IClockifyItIntegration, ITransientDependency
    {
        public const string Name = "RedmineOverDevOps@0";

        public ILogger<RedmineOverDevOpsIntegration> Logger { get; set; }

        public RedmineOverDevOpsIntegration()
        {
            Logger = NullLogger<RedmineOverDevOpsIntegration>.Instance;
        }

        public async Task<IntegrationResult> ProcessAsync(IntegrationContext context)
        {
            var options = context.Configuration.Get<RedmineOverDevOpsIntegrationOptions>();

            var issueMap = ExtractIssueMap(context, options);
            var workItemIds = issueMap.GetDistinctReferencedIssueIds();
            var devOpsClientFactory = context.ServiceProvider.GetRequiredService<DevOpsClientFactory>();
            var devOpsClient = devOpsClientFactory.Create(options.Host, options.PAT);
            var manager = GetRedmineManager(options);

            var result = new IntegrationResult();

            foreach (var workItemId in workItemIds)
            {
                var timeEntryIds = issueMap.GetTimeEntriesByIssueId(workItemId);
                var timeEntries = context.TimeEntries.Where(x => timeEntryIds.Contains(x.Id)).ToArray();
                try
                {
                    var workItem = await devOpsClient.GetWorkItemAsync(workItemId);
                    var redmineIssueId = GetReferencedRedmineIssueId(workItem, options);

                    var issue = await GetIssueAsync(manager, redmineIssueId);

                    foreach (var timeEntry in timeEntries)
                    {
                        try
                        {
                            await CreateTimeBookingAsync(manager, issue, timeEntry);

                            result.MarkAsProcessed(timeEntry.Id);
                        }
                        catch (Exception individualEntryException)
                        {
                            // We failed at processing a specific entry, set it in a failed state
                            // and attempt to process other entries referencing the current issue.
                            result.MarkAsFailed(timeEntry.Id, individualEntryException);
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

        protected virtual int GetReferencedRedmineIssueId(WorkItem workItem, RedmineOverDevOpsIntegrationOptions options)
        {
            if (workItem.Relations == null)
            {
                throw new BusinessException("Failed to find redmine issue hyperlink. Please add one.");
            }

            var redmineUrl = options.Redmine.Host.EnsureEndsWith('/');
            redmineUrl += "issues/";

            var hyperlinks = workItem.Relations
                .Where(x => x.Rel == "Hyperlink")
                .Where(x => x.Url.Contains(redmineUrl));

            if (hyperlinks.Count() > 1)
            {
                hyperlinks = hyperlinks.Where(x => x.Title == options.KeyComment);
            }

            if (hyperlinks.Count() > 1)
            {
                throw new BusinessException("Ambiguous links in work-item. Please clarify your links.");
            }
            else if (hyperlinks.Count() == 0)
            {
                throw new BusinessException("Failed to find redmine issue hyperlink. Please add one.");
            }

            var hyperlink = hyperlinks.FirstOrDefault();

            var matches = Regex.Match(hyperlink.Url, options.RedmineUrlIssueIdExpression, RegexOptions.IgnoreCase);
            if (matches.Groups.Count == 2)
            {
                var issueId = matches.Groups[1].Value;

                if (int.TryParse(issueId, out var redmineIssueId))
                {
                    return redmineIssueId;
                }
            }

            throw new BusinessException("Failed to parse redmine issue id from hyperlink.")
                .WithData("Url", hyperlink.Url);
        }

        protected virtual TimeEntryMap<int> ExtractIssueMap(IntegrationContext context, RedmineOverDevOpsIntegrationOptions options)
        {
            var issueMap = new TimeEntryMap<int>();
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

        protected virtual RedmineManager GetRedmineManager(RedmineOverDevOpsIntegrationOptions options)
        {
            return new RedmineManager(options.Redmine.Host, options.Redmine.ApiKey, verifyServerCert: options.Redmine.VerifyServerCert);
        }

        protected virtual async Task<Issue> GetIssueAsync(RedmineManager manager, int id)
        {
            return await manager.GetObjectAsync<Issue>(id.ToString(), new NameValueCollection());
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
