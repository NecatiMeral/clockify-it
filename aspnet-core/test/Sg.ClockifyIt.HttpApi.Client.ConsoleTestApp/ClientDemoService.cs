using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Sg.ClockifyIt.HttpApi.Client.ConsoleTestApp;

public class ClientDemoService : ITransientDependency
{
    public ClientDemoService()
    {
    }

    public Task RunAsync()
    {
        return Task.CompletedTask;
    }
}
