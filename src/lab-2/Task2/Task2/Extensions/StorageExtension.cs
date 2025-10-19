using Microsoft.Extensions.DependencyInjection;
using Task2.Sources.Providers;

namespace Task2.Extensions;

public static class StorageExtension
{
    public static IServiceCollection IncludeStorage(this IServiceCollection services)
    {
        services.AddSingleton<ConfigStorage, ConfigStorage>();
        return services;
    }
}