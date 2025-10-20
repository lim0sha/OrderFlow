using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Task2.Utils;

namespace Task2.Extensions;

public static class UpdaterOptionsExtension
{
    public static IServiceCollection IncludeUpdaterOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<UpdaterOptions>()
            .Bind(configuration.GetSection("ConfigUpdater"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
