using System.Collections.Generic;
using System.Linq;

namespace Sg.ClockifyIt.Integrations.Redmine
{
    public class TimeEntryToIssueMap
    {
        readonly Dictionary<string, List<string>> _timeEntryMap;

        public TimeEntryToIssueMap()
        {
            _timeEntryMap = new Dictionary<string, List<string>>();
        }

        public void Map(string timeEntryId, IEnumerable<string> issueIds)
        {
            if (_timeEntryMap.ContainsKey(timeEntryId))
            {
                _timeEntryMap[timeEntryId].AddRange(issueIds);
            }
            _timeEntryMap[timeEntryId] = issueIds.ToList();
        }

        public string[] GetDistinctReferencedIssueIds()
        {
            return _timeEntryMap.Values.SelectMany(x => x)
                .Distinct()
                .ToArray();
        }

        public string[] GetTimeEntriesByIssueId(string issueId)
        {
            return _timeEntryMap.Where(x => x.Value.Contains(issueId))
                .Select(x => x.Key)
                .ToArray();
        }
    }
}
