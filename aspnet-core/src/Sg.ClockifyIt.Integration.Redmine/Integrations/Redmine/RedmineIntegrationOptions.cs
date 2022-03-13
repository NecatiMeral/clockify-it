using Sg.ClockifyIt.Integrations;

namespace Sg.ClockifyIt.Integrations.Redmine
{
    public class RedmineIntegrationOptions : CommonIntegrationOptions
    {
        public string Host { get; set; }

        public string ApiKey { get; set; }

        public bool VerifyServerCert { get; set; }

        public string IssueIdExpression { get; set; }

        public RedmineIntegrationOptions()
        {
            VerifyServerCert = true;
            IssueIdExpression = "#([0-9]+)";
        }
    }
}
