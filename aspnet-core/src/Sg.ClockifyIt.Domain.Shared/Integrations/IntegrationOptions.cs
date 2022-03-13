using System;
using System.Collections.Generic;

namespace Sg.ClockifyIt.Integrations
{
    public class IntegrationOptions
    {
        public IDictionary<string, Type> Integrations { get; set; }

        public IntegrationOptions()
        {
            Integrations = new Dictionary<string, Type>();
        }
    }
}
