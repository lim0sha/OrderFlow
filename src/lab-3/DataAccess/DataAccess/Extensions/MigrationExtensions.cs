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
        services.AddSingleton<IMigrationRunner>(sp =>
        {
            DbConnectionConfig config = sp.GetRequiredService<IOptions<DbConnectionConfig>>().Value;
            string connectionString = DbConnectionStringBuilder.Build(config);

            IMigrationRunner runner = new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddPostgres()
                    .WithGlobalConnectionString(connectionString)
                    .WithMigrationsIn(typeof(InitialMigration).Assembly))
                .BuildServiceProvider()
                .GetService<IMigrationRunner>() ?? throw new InvalidOperationException(
                "Error in IMigrationRunner.");
            return runner;
        });

        return services;
    }
}