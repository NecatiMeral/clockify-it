using System;

namespace Sg.ClockifyIt.Integrations.Dtos
{
    public class TimeIntervalDto
    {
        public string Duration { get; set; }

        public DateTimeOffset? End { get; set; }

        public DateTimeOffset? Start { get; set; }
    }
}
