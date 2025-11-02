using DataAccess.Extensions;
using DataAccess.Models.Entities.Common.ResultTypes;
using DataAccess.Models.Entities.Orders;
using DataAccess.Models.Entities.Products;
using DataAccess.Models.Enums;
using DataAccess.Models.Requests;
using DataAccess.Repositories.Interfaces;
using DataAccess.Services.Interfaces;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DataAccess.Console;

internal abstract class Program
{
#pragma warning disable CA1506
    private static async Task Main(string[] args)
#pragma warning restore CA1506
    {
        AppContext.SetSwitch("System.Transactions.EnableAsyncFlow", true);
        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((_, builder) =>
            {
                builder
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddDbConnectionConfig(context.Configuration);
                services.AddRepositories();
                services.AddApplicationServices();
                services.AddMigrationsFromOptions();
            })
            .Build();

        using IServiceScope scope = host.Services.CreateScope();
        IMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();

        IProductService productService = scope.ServiceProvider.GetRequiredService<IProductService>();
        IOrderService orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
        IOrderHistoryRepository orderHistoryRepo = scope.ServiceProvider.GetRequiredService<IOrderHistoryRepository>();

        CancellationToken ct = CancellationToken.None;

        try
        {
            ProductOperationResult laptopResult = await productService.Create(new Product(0, "Laptop", 1200.0m), ct);
            ProductOperationResult mouseResult = await productService.Create(new Product(0, "Mouse", 25.5m), ct);

            if (laptopResult.GetErrorType() is not null || mouseResult.GetErrorType() is not null)
                throw new Exception("Failed to create products");

            long laptopId = ((ProductOperationResult.Success)laptopResult).ProductId;
            long mouseId = ((ProductOperationResult.Success)mouseResult).ProductId;

            if (laptopId == 0 || mouseId == 0)
                throw new Exception("Product IDs cannot be 0");

            System.Console.WriteLine($"[INFO]: Created products: Laptop (ID={laptopId}), Mouse (ID={mouseId})");

            OrderOperationResult orderResult = await orderService.Create(
                new Order(0, OrderState.Created, DateTime.UtcNow, "test-user"),
                ct);

            if (orderResult is not OrderOperationResult.Success orderSuccess)
                throw new Exception("Failed to create order");

            long orderId = orderSuccess.OrderItemId ?? throw new Exception("Failed to get order ID");

            System.Console.WriteLine($"[INFO]: Created order ID={orderId}");

            OrderOperationResult addItem1 = await orderService.AddItem(
                new OrderItem(0, orderId, laptopId, 1, false),
                ct);
            OrderOperationResult addItem2 = await orderService.AddItem(
                new OrderItem(0, orderId, mouseId, 2, false),
                ct);

            if (addItem1.GetErrorType() is not null || addItem2.GetErrorType() is not null)
                throw new Exception("Failed to add items");

            long mouseOrderItemId = ((OrderOperationResult.Success)addItem2).OrderItemId ??
                                    throw new Exception("Mouse order item ID is null");

            System.Console.WriteLine($"[INFO]: Added items; mouse item ID = {mouseOrderItemId}");

            OrderOperationResult removeResult = await orderService.RemoveItem(orderId, mouseOrderItemId, ct);
            if (removeResult.GetErrorType() is not null)
                throw new Exception("Failed to remove item");
            System.Console.WriteLine("[INFO]: Removed mouse from order");

            OrderOperationResult transferResult = await orderService.TransferToWork(orderId, ct);
            if (transferResult.GetErrorType() is not null)
                throw new Exception("Failed to transfer to work");
            System.Console.WriteLine("[INFO]: Order transferred to 'processing'");

            OrderOperationResult completeResult = await orderService.CompleteOrder(orderId, ct);
            if (completeResult.GetErrorType() is not null)
                throw new Exception("Failed to complete order");
            System.Console.WriteLine("[INFO]: Order completed");

            System.Console.WriteLine($"\nFull history for order {orderId}:");
            await foreach (OrderHistory history in orderHistoryRepo.GetFiltered(
                               position: 0,
                               volume: 100,
                               request: new OrderHistoryRequestFiltered(orderId, OrderHistoryItemKind.ItemAdded),
                               ct))
            {
                System.Console.WriteLine(
                    $"  [{history.OrderHistoryItemCreatedAt:HH:mm:ss}] {history.OrderHistoryItemKind}");
            }

            System.Console.WriteLine("\nSuccess!");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error: {ex}");
            Environment.Exit(1);
        }
    }
}