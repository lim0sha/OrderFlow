using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Spectre.Console;
using Task3.Extensions;
using Task3.Models.Entities;
using Task3.Services.Interfaces;

namespace Task3.Console;

public static class Program
{
    private static async Task Main()
    {
        using IHost host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((_, builder) => builder
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(@"D:\ITMO\5_SEM\CSMS\lim0sha\src\lab-2\Task1\Task1\serviceconfig.json", optional: false, reloadOnChange: true))
            .ConfigureServices((context, collection) =>
            {
                IConfiguration config = context.Configuration;
                collection.IncludeConfig(config);
            })
            .Build();

        IOptionsMonitor<RendererBlueprint> optionsMonitor =
            host.Services.GetRequiredService<IOptionsMonitor<RendererBlueprint>>();
        IDisplayer displayer = host.Services.GetRequiredService<IDisplayer>();

        AnsiConsole.Clear();
        displayer.Display(optionsMonitor.CurrentValue);

        optionsMonitor.OnChange(snapshot =>
        {
            AnsiConsole.Clear();
            displayer.Display(snapshot);
        });

        await host.RunAsync();
    }
}