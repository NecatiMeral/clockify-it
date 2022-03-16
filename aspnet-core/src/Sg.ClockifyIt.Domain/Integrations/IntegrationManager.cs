using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sg.ClockifyIt.Integrations.Dtos;
using Sg.ClockifyIt.Sync.Users;
using Sg.ClockifyIt.Workspaces;
using Volo.Abp.DependencyInjection;

namespace Sg.ClockifyIt.Integrations
{
    public class IntegrationManager : ITransientDependency
    {
        protected IntegrationOptions Options { get; }
        protected IServiceScopeFactory ServiceScopeFactory { get; }
        protected IConfiguration Configuration { get; }

        public IntegrationManager(IServiceScopeFactory serviceScopeFactory,
            IOptions<IntegrationOptions> options,
            IConfiguration configuration)
        {
            ServiceScopeFactory = serviceScopeFactory;
            Configuration = configuration;
            Options = options.Value;
        }

        public virtual async Task<IntegrationResultAggregate> RunIntegrationsAsync(string workspaceId, WorkspaceConfiguration workspace, UserInfo user, List<TimeEntryDto> timeEntries)
        {
            using var scope = ServiceScopeFactory.CreateScope();

            var result = new IntegrationResultAggregate();
            foreach (var integrationReference in workspace.Integrations)
            {
                var integrationConfiguration = GetIntegrationConfiguration(integrationReference.Name);
                var type = GetIntegrationType(integrationConfiguration["Name"]);
                var integration = (IClockifyItIntegration)scope.ServiceProvider.GetRequiredService(type);

                var context = new IntegrationContext(
                    scope.ServiceProvider,
                    integrationConfiguration.GetSection("Args"),
                    user,
                    workspace,
                    workspaceId,
                    timeEntries = new List<TimeEntryDto>(timeEntries)
                );

                var integrationResult = await integration.ProcessAsync(context);

                result.AddActions(integrationResult.CompletionActions);
                foreach (var item in integrationResult)
                {
                    result.Set(item.Key, item.Value);
                }
            }
            return result;
        }

        protected virtual IConfiguration GetIntegrationConfiguration(string integrationName)
        {
            return Configuration
                ?.GetSection(ClockifyConfigurationConsts.ClockifyConfigurationSectionName)
                ?.GetSection(ClockifyConfigurationConsts.IntegrationsConfigurationSectionName)
                ?.GetSection(integrationName);
        }

        protected virtual Type GetIntegrationType(string name)
        {
            if (!Options.Integrations.TryGetValue(name, out var integrationType))
            {
                throw new NotImplementedException($"Unknown integration `{name}`");
            }

            if (!typeof(IClockifyItIntegration).IsAssignableFrom(integrationType))
            {
                throw new InvalidOperationException(
                    $"Integration type `{integrationType}` is not assignable to `{nameof(IClockifyItIntegration)}`"
                );
            }

            return integrationType;
        }
    }
}
