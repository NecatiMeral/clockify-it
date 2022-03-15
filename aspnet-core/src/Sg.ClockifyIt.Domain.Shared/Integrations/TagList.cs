using System.Collections.Generic;
using System.Linq;

namespace Sg.ClockifyIt.Integrations
{
    public class TagList : List<string>
    {
        public bool MatchesTags(IEnumerable<string> tags)
        {
            return TrueForAll(x => tags.Contains(x));
        }
    }
}
