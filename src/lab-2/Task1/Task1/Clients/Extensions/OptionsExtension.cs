using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Task1.Models.Entities;

namespace Task1.Clients.Extensions;

public static class OptionsExtension
{
    public static IServiceCollection IncludeOptions(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddOptions<ConnectionOptions>()
            .Bind(config.GetSection("ServiceConnection:Settings"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}