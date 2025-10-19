using Microsoft.Extensions.DependencyInjection;
using Task2.Sources.Managers;
using Task2.Sources.Managers.Interfaces;

namespace Task2.Extensions;

public static class ManagerExtension
{
    public static IServiceCollection IncludeManager(this IServiceCollection services)
    {
        services.AddScoped<IConfigManager, ConfigSyncManager>();
        return services;
    }
}