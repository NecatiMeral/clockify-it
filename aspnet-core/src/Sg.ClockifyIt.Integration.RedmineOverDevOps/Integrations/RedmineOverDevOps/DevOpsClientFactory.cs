using Volo.Abp.DependencyInjection;

namespace Sg.ClockifyIt.Integrations.RedmineOverDevOps
{
    public class DevOpsClientFactory : ITransientDependency
    {
        public DevOpsClient Create(string host, string accessToken)
        {
            return new DevOpsClient(host, accessToken);
        }
    }
}
