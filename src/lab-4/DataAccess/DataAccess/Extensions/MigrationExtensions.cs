using DataAccess.Configs;
using DataAccess.Migrations;
using DataAccess.Utils.DbUtils;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DataAccess.Extensions;

public static class MigrationExtensions
{
    public static IServiceCollection AddMigrationsFromOptions(this IServiceCollection services)
    {
        services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(sp =>
                {
                    DbConnectionConfig config = sp.GetRequiredService<IOptions<DbConnectionConfig>>().Value;
                    return DbConnectionStringBuilder.Build(config);
                })
                .WithMigrationsIn(typeof(InitialMigration).Assembly));

        return services;
    }
}