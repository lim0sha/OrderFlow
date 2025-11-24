using DataAccess.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Presentation.Extensions;
using Presentation.Services;
using Task1.Clients.Extensions;
using Task2.Extensions;

namespace Presentation.Application;

internal abstract class Program
{
    private static Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Configuration
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        Console.WriteLine($"[DEBUG] Current directory: {Directory.GetCurrentDirectory()}");
        Console.WriteLine($"[DEBUG] AppContext.BaseDirectory: {AppContext.BaseDirectory}");

        builder.Logging.AddConsole();

        builder.Services
            .IncludeOptions(builder.Configuration)
            .IncludeUpdaterOptions(builder.Configuration)
            .IncludeDefaultHttpClient()
            .IncludeStorage()
            .IncludeCustomSource()
            .IncludeManager()
            .AddDbConnectionConfig(builder.Configuration);

        builder.Services
            .AddGrpcServices()
            .AddRepositories()
            .AddApplicationServices()
            .AddMigrationsFromOptions();

        WebApplication app = builder.Build();

        app.MapGrpcService<ProductGrpcService>();
        app.MapGrpcService<OrderGrpcService>();

        app.Run();
        return Task.CompletedTask;
    }
}