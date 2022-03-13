using System.Threading.Tasks;
using Sg.ClockifyIt.Integrations;

namespace Sg.ClockifyIt.Integrations
{
    public interface IClockifyItIntegration
    {
        Task<IntegrationResult> ProcessAsync(IntegrationContext context);
    }
}
