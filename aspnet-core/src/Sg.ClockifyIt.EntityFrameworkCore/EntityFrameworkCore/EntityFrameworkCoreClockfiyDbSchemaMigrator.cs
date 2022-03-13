using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sg.ClockifyIt.Data;
using Volo.Abp.DependencyInjection;

namespace Sg.ClockifyIt.EntityFrameworkCore;

public class EntityFrameworkCoreClockifyItDbSchemaMigrator
    : IClockifyItDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreClockifyItDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the ClockifyItDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<ClockifyItDbContext>()
            .Database
            .MigrateAsync();
    }
}
