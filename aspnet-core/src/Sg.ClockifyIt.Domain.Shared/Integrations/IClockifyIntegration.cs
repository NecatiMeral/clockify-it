using System.Threading.Tasks;
using Sg.ClockifyIt.Integrations;

namespace Sg.ClockifyIt.Integrations
{
    public interface IClockifyItIntegration
    {
        Task<IntegrationProcessingResult> ProcessAsync(IntegrationContext context);
    }
}
