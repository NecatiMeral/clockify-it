using System.Collections.Generic;

namespace Sg.ClockifyIt.Integrations
{
    public class CommonIntegrationOptions
    {
        public List<string> Tags { get; set; }

        public List<string> ProcessedTags { get; set; }

        public List<string> ErrorTags { get; set; }

        public CommonIntegrationOptions()
        {
            Tags = new List<string>();
            ProcessedTags = new List<string>();
            ErrorTags = new List<string>();
        }
    }
}
