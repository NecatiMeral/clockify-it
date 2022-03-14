using Sg.ClockifyIt.Integrations.Redmine;

namespace Sg.ClockifyIt.Integrations.RedmineOverDevOps
{
    public class RedmineOverDevOpsIntegrationOptions : CommonIntegrationOptions
    {
        public RedmineIntegrationOptions Redmine { get; set; }

        public string IssueIdExpression { get; set; }

        public string Host { get; set; }

        public string PAT { get; set; }

        public string KeyComment { get; set; }

        public string RedmineUrlIssueIdExpression { get; set; }

        public RedmineOverDevOpsIntegrationOptions()
        {
            Redmine = new RedmineIntegrationOptions();
            IssueIdExpression = "#([0-9]+)";
            KeyComment = "Clockify";
            RedmineUrlIssueIdExpression = @"\/issues\/(\d+)(?:\.+)?";
        }
    }
}
