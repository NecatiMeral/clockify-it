using System.Collections.Generic;

namespace Sg.ClockifyIt.Integrations
{
    public class CommonIntegrationOptions
    {
        public List<string> Tags { get; set; }

        public CommonIntegrationOptions()
        {
            Tags = new List<string>();
        }
    }
}
