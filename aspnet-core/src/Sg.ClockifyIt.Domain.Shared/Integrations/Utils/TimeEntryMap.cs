using System.Collections.Generic;
using System.Linq;

namespace Sg.ClockifyIt.Integrations.Redmine
{
    public class TimeEntryMap<TIssueKey>
    {
        readonly Dictionary<string, List<TIssueKey>> _timeEntryMap;

        public TimeEntryMap()
        {
            _timeEntryMap = new Dictionary<string, List<TIssueKey>>();
        }

        public void Map(string timeEntryId, IEnumerable<TIssueKey> issueIds)
        {
            if (_timeEntryMap.ContainsKey(timeEntryId))
            {
                _timeEntryMap[timeEntryId].AddRange(issueIds);
            }
            _timeEntryMap[timeEntryId] = issueIds.ToList();
        }

        public TIssueKey[] GetDistinctReferencedIssueIds()
        {
            return _timeEntryMap.Values.SelectMany(x => x)
                .Distinct()
                .ToArray();
        }

        public string[] GetTimeEntriesByIssueId(TIssueKey issueId)
        {
            return _timeEntryMap.Where(x => x.Value.Contains(issueId))
                .Select(x => x.Key)
                .ToArray();
        }
    }
}
