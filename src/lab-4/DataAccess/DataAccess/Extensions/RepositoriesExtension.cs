using DataAccess.Configs;
using DataAccess.Models.Entities.Orders;
using DataAccess.Models.Enums;
using DataAccess.Repositories.Implementation;
using DataAccess.Repositories.Interfaces;
using DataAccess.Utils.DbUtils;
using DataAccess.Utils.Helpers;
using DataAccess.Utils.Helpers.Interfaces;
using DataAccess.Utils.Mappers;
using DataAccess.Utils.Mappers.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;

namespace DataAccess.Extensions;

public static class RepositoriesExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton(serviceProvider =>
        {
            IOptions<DbConnectionConfig> dbConfig = serviceProvider.GetRequiredService<IOptions<DbConnectionConfig>>();
            string connectionString = DbConnectionStringBuilder.Build(dbConfig.Value);
            var builder = new NpgsqlDataSourceBuilder(connectionString);
            builder.MapEnum<OrderState>(pgName: "order_state");
            builder.MapEnum<OrderHistoryItemKind>(pgName: "order_history_item_kind");

            NpgsqlDataSource dataSource = builder.Build();
            return dataSource;
        });

        services.AddSingleton<IDbCommandExecutor, DbCommandExecutor>();

        services.AddSingleton<IOrderMapper, OrderMapper>();
        services.AddSingleton<IOrderItemMapper, OrderItemMapper>();
        services.AddSingleton<IOrderHistoryMapper, OrderHistoryMapper>();

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IOrderHistoryRepository, OrderHistoryRepository>();

        return services;
    }
}