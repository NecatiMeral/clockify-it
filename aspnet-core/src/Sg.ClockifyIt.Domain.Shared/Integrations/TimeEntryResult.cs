using System;

namespace Sg.ClockifyIt.Integrations
{
    public class TimeEntryResult
    {
        public bool Succeed { get; set; }

        public Exception Exception { get; set; }

        public TimeEntryResult()
        {
            Succeed = true;
        }

        public TimeEntryResult(Exception exception)
        {
            Succeed = false;
            Exception = exception;
        }
    }
}
