using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Sg.ClockifyIt.Sync;
using Sg.ClockifyIt.Workspaces;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace Sg.ClockifyIt
{
    public class ClockifyItBackgroundWorker : AsyncPeriodicBackgroundWorkerBase
    {
        protected IHostApplicationLifetime HostLifetime { get; }
        protected WorkspaceOptions Options { get; }

        public ClockifyItBackgroundWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory, IHostApplicationLifetime hostLifetime, IOptions<WorkspaceOptions> options)
            : base(timer, serviceScopeFactory)
        {
            Options = options.Value;
            HostLifetime = hostLifetime;

            Timer.Period = (int)Options.Interval.TotalMilliseconds;
            Timer.RunOnStart = true;
        }

        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            var manager = workerContext.ServiceProvider.GetRequiredService<SyncManager>();

            await manager.SyncAsync();

            if (Options.Once)
            {
                // This doesn't seem to do the trick
                //HostLifetime.StopApplication();
                Environment.Exit(0);
            }
        }
    }
}
