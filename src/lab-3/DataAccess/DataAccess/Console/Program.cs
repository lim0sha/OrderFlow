using DataAccess.Extensions;
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
        IProductRepository productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
        IOrderRepository orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

        using var cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;

        try
        {
            bool laptopResult = await productService.Create(new Product(0, "Laptop", 1200.0m), ct);
            bool mouseResult = await productService.Create(new Product(0, "Mouse", 25.5m), ct);

            if (!laptopResult || !mouseResult)
                throw new Exception("Failed to create products");

            Product laptop = await productRepository.GetProductByName("Laptop", ct);
            Product mouse = await productRepository.GetProductByName("Mouse", ct);

            long laptopId = laptop.Id;
            long mouseId = mouse.Id;

            if (laptopId == 0 || mouseId == 0)
                throw new Exception("Product IDs cannot be 0");

            System.Console.WriteLine($"[INFO]: Created products: Laptop (ID={laptopId}), Mouse (ID={mouseId})");

            bool orderResult = await orderService.Create(new Order(0, OrderState.Created, DateTime.UtcNow, "test-user"), ct);

            if (!orderResult)
                throw new Exception("Failed to create order");

            Order order = await orderRepository.GetOrderByUser("test-user", ct);
            long orderId = order.Id;

            if (orderId == 0)
                throw new Exception("Failed to get order ID");

            System.Console.WriteLine($"[INFO]: Created order ID={orderId}");

            bool addItem1 = await orderService.AddItem(new OrderItem(0, orderId, laptopId, 1, false), ct);
            bool addItem2 = await orderService.AddItem(new OrderItem(0, orderId, mouseId, 2, false), ct);

            if (!addItem1 || !addItem2)
                throw new Exception("Failed to add items");

            OrderItem mouseOrderItem = await orderRepository.GetOrderItemByProduct(mouseId, orderId, ct);
            long mouseOrderItemId = mouseOrderItem.Id;

            if (mouseOrderItemId == 0)
                throw new Exception("Mouse order item ID is null");

            System.Console.WriteLine($"[INFO]: Added items; mouse item ID = {mouseOrderItemId}");

            bool removeResult = await orderService.RemoveItem(orderId, mouseOrderItemId, ct);
            if (!removeResult)
                throw new Exception("Failed to remove item");
            System.Console.WriteLine("[INFO]: Removed mouse from order");

            bool transferResult = await orderService.TransferToWork(orderId, ct);
            if (!transferResult)
                throw new Exception("Failed to transfer to work");
            System.Console.WriteLine("[INFO]: Order transferred to 'processing'");

            bool completeResult = await orderService.CompleteOrder(orderId, ct);
            if (!completeResult)
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