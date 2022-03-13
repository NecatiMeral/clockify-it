using Clockify.Net;
using Sg.ClockifyIt.ClockifyIt.Cache;
using Sg.ClockifyIt.Workspaces;
using Volo.Abp.DependencyInjection;

namespace Sg.ClockifyIt.ClockifyIt
{
    internal class ClockifyItClientFactory : IClockifyItClientFactory, IScopedDependency
    {
        private readonly ClockifyItClientCache _clientCache;

        public ClockifyItClientFactory(ClockifyItClientCache clientCache)
        {
            _clientCache = clientCache;
        }

        public ClockifyClient CreateClient(WorkspaceConfiguration workspaceConfiguration)
        {
            return _clientCache.GetOrAdd(
                workspaceConfiguration.ApiKey,
                () => CreateNewClient(workspaceConfiguration)
            );
        }

        protected virtual ClockifyClient CreateNewClient(WorkspaceConfiguration workspaceConfiguration)
        {
            return new ClockifyClient(
                workspaceConfiguration.ApiKey,
                workspaceConfiguration.ApiUrl,
                workspaceConfiguration.ExperimentalApiUrl,
                workspaceConfiguration.ReportsApiUrl
            );
        }
    }
}
