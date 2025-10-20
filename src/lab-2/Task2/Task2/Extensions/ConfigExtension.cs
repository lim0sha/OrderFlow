using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Task2.Sources;

namespace Task2.Extensions;

public static class ConfigExtension
{
    public static IServiceCollection IncludeCustomSource(this IServiceCollection services)
    {
        services.AddSingleton<IConfigurationSource, MyConfigEntitySource>();
        return services;
    }
}