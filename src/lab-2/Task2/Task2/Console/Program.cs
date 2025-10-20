using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Task1.Clients.Extensions;
using Task2.Extensions;
using Task2.Sources.Managers.Interfaces;

namespace Task2.Console;

internal abstract class Program
{
    private static async Task Main()
    {
        using IHost host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((_, builder) => builder
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("serviceconfig.json", optional: false, reloadOnChange: true))
            .ConfigureServices((context, serviceCollection) =>
            {
                IConfiguration config = context.Configuration;

                serviceCollection.IncludeOptions(config);
                serviceCollection.IncludeUpdaterOptions(config);
                serviceCollection.IncludeDefaultHttpClient();
                serviceCollection.IncludeRefitClient();

                serviceCollection.IncludeStorage();
                serviceCollection.IncludeCustomSource();
                serviceCollection.IncludeManager();
            })
            .Build();

        IConfigManager configManager = host.Services.GetRequiredService<IConfigManager>();

        using var cts = new CancellationTokenSource();
        await configManager.Update(2, null, cts.Token);

        System.Console.WriteLine("Configuration synchronized 1 time");

        _ = Task.Run(async () => await configManager.UpdateOnInterval(25, null, cts.Token));

        System.Console.WriteLine("Periodic update is every N sec");
        System.Console.WriteLine("Press Enter to stop");

        System.Console.ReadLine();
        await cts.CancelAsync();
    }
}