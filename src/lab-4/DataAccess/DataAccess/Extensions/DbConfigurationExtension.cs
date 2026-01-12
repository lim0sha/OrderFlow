using DataAccess.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Extensions;

public static class DbConfigurationExtension
{
    public static IServiceCollection AddDbConnectionConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DbConnectionConfig>(
            configuration.GetSection("DbConnection"));
        return services;
    }
}