using Confluent.Kafka;
using DataAccess.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orders.Kafka.Contracts;
using Presentation.Extensions;
using Presentation.Kafka.Abstractions.Extensions;
using Presentation.Kafka.Abstractions.Options;
using Presentation.Kafka.Serializers;
using Presentation.Services;
using Task1.Clients.Extensions;
using Task2.Extensions;

namespace Presentation.Application;

internal abstract class Program
{
#pragma warning disable CA1506
    private static Task Main(string[] args)
#pragma warning restore CA1506
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Configuration
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

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
            .IncludeGrpcServices()
            .AddRepositories()
            .AddApplicationServices()
            .AddMigrationsFromOptions();

        builder.Services.AddSingleton(Serializers.Int64);
        builder.Services
            .AddSingleton<ISerializer<OrderCreationValue>, ProtobufSerializer<OrderCreationValue>>();

        builder.Services.AddKafkaProducer(
            topic: "order_creation",
            keySerializer: builder.Services.BuildServiceProvider().GetRequiredService<ISerializer<long>>(),
            valueSerializer: builder.Services.BuildServiceProvider()
                .GetRequiredService<ISerializer<OrderCreationValue>>());

        builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection("Kafka"));

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(8080, o => o.Protocols = HttpProtocols.Http1);
            options.ListenAnyIP(5000, o => o.Protocols = HttpProtocols.Http2);
        });

        WebApplication app = builder.Build();

        app.MapGrpcService<ProductGrpcService>();
        app.MapGrpcService<OrderGrpcService>();

        app.Run();
        return Task.CompletedTask;
    }
}