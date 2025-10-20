using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Task1.Clients;
using Task1.Clients.Extensions;
using Task1.Clients.Interfaces;
using Task1.Models.Entities;

namespace Task1.Console;

internal abstract class Program
{
    private static async Task Main()
    {
        var services = new ServiceCollection();

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("serviceconfig.json", optional: false, reloadOnChange: true)
            .Build();

        services.IncludeOptions(configuration);
        services.IncludeDefaultHttpClient();
        services.IncludeRefitClient();

        ServiceProvider serviceProvider = services.BuildServiceProvider();

        IWebClient httpClient = serviceProvider.GetRequiredService<IWebClient>();
        RefitClient refitClient = serviceProvider.GetRequiredService<RefitClient>();

        string? pageToken = null;
        int pageNumber = 1;

        System.Console.WriteLine("DefaultHttpClientClient:");
        await foreach (Config config in httpClient.FetchConfigs(50, pageToken, CancellationToken.None))
        {
            System.Console.WriteLine($"Page {pageNumber} | Key: {config.Key}, Value: {config.Value}");
        }

        pageToken = null;
        pageNumber = 1;

        System.Console.WriteLine("RefitClient:");
        await foreach (Config config in refitClient.FetchConfigs(50, pageToken, CancellationToken.None))
        {
            System.Console.WriteLine($"Page {pageNumber} | Key: {config.Key}, Value: {config.Value}");
        }
    }
}