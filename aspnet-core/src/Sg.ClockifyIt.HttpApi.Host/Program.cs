using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Sg.ClockifyIt;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File("Logs/logs.log", retainedFileCountLimit: 14, rollingInterval: RollingInterval.Day))
            .WriteTo.Async(c => c.Console())
            .CreateBootstrapLogger();

        try
        {
            Log.Information("Starting Sg.ClockifyIt.HttpApi.Host.");
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.AddAppSettingsSecretsJson()
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddUserSecrets<Program>();
                })
                .UseAutofac()
                .UseSerilog((context, configuration) =>
                {
                    configuration
                        .WriteTo.Async(x => x.Console())
                        .ReadFrom.Configuration(context.Configuration);
                });

            await builder.AddApplicationAsync<ClockifyItHttpApiHostModule>();
            var app = builder.Build();
            await app.InitializeApplicationAsync();
            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
