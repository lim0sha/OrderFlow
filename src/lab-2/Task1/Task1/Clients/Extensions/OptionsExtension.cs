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
            .Validate(options =>
            {
                if (string.IsNullOrWhiteSpace(options.ConnectionHost))
                    return false;

                return options.ConnectionPort is > 0 and <= 65535;
            }).ValidateOnStart();

        return services;
    }
}