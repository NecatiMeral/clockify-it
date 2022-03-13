using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sg.ClockifyIt.Sync;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace Sg.ClockifyIt
{
    public class ClockifyItBackgroundWorker : AsyncPeriodicBackgroundWorkerBase
    {
        public ClockifyItBackgroundWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory)
            : base(timer, serviceScopeFactory)
        {
            Timer.Period = (int)TimeSpan.FromMinutes(10).TotalMilliseconds;
            Timer.RunOnStart = true;
        }

        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            var manager = workerContext.ServiceProvider.GetRequiredService<SyncManager>();

            await manager.SyncAsync();
        }
    }
}
