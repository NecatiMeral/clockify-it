using System.Threading.Tasks;

namespace Sg.ClockifyIt.Data;

public interface IClockifyItDbSchemaMigrator
{
    Task MigrateAsync();
}
