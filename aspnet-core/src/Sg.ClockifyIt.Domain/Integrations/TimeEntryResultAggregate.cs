using System;
using System.Collections.Generic;

namespace Sg.ClockifyIt.Integrations
{
    public class TimeEntryResultAggregate
    {
        public bool Succeed { get; set; }

        public HashSet<Exception> Exceptions { get; set; }

        public TimeEntryResultAggregate()
        {
            Succeed = true;
            Exceptions = new HashSet<Exception>();
        }
    }
}
