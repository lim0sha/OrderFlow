using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Task3.Extensions;

namespace Task3.Console;

public static class Program
{
    private static async Task Main()
    {
        using IHost host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((_, builder) => builder
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(
                    @"D:\ITMO\5_SEM\CSMS\lim0sha\src\lab-2\Task1\Task1\serviceconfig.json",
                    optional: false,
                    reloadOnChange: true))
            .ConfigureServices((context, collection) =>
            {
                IConfiguration config = context.Configuration;
                collection.AddServices(config);
            })
            .Build();
        await host.RunAsync();
    }
}