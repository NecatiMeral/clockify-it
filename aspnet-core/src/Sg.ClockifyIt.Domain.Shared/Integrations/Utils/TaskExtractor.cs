using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sg.ClockifyIt.Integrations.Utils
{
    public class TaskExtractor
    {
        public static ICollection<string> ExtractTaskFromDescription(string description, string regularExpressionPattern)
        {
            var matches = Regex.Matches(description, regularExpressionPattern);

            var tasks = new HashSet<string>();
            foreach (Match match in matches)
            {
                tasks.Add(match.Groups[1].Value);
            }
            return tasks;
        }
    }
}
