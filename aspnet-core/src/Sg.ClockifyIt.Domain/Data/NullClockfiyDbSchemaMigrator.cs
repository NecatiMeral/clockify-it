using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Sg.ClockifyIt.Data;

/* This is used if database provider does't define
 * IClockifyItDbSchemaMigrator implementation.
 */
public class NullClockifyItDbSchemaMigrator : IClockifyItDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
