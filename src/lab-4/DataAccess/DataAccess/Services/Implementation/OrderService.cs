using DataAccess.Models.Entities.Orders;
using DataAccess.Models.Enums;
using DataAccess.Models.Requests;
using DataAccess.Repositories.Interfaces;
using DataAccess.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Transactions;

namespace DataAccess.Services.Implementation;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderHistoryRepository _orderHistoryRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepository,
        IOrderHistoryRepository orderHistoryRepository,
        IOrderItemRepository orderItemRepository,
        ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _orderHistoryRepository = orderHistoryRepository;
        _orderItemRepository = orderItemRepository;
        _logger = logger;
    }

    public async Task<bool> Create(Order o, CancellationToken ct)
    {
        using TransactionScope transaction = InitBaseTransaction();

        try
        {
            long orderId = await _orderRepository.Create(o, ct);
            var history = new OrderHistory(
                Id: 0,
                OrderId: orderId,
                OrderHistoryItemCreatedAt: DateTime.UtcNow,
                OrderHistoryItemKind: OrderHistoryItemKind.Created,
                OrderHistoryItemPayload: null);
            await _orderHistoryRepository.Create(history, ct);
            transaction.Complete();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create order");
            return false;
        }
    }

    public async Task<bool> AddItem(OrderItem oi, CancellationToken ct)
    {
        using TransactionScope transaction = InitBaseTransaction();

        try
        {
            Order order = await _orderRepository.GetById(oi.OrderId, ct);

            if (order.OrderState != OrderState.Created)
                return false;

            await _orderItemRepository.Create(oi, ct);
            var history = new OrderHistory(
                Id: 0,
                OrderId: oi.OrderId,
                OrderHistoryItemCreatedAt: DateTime.UtcNow,
                OrderHistoryItemKind: OrderHistoryItemKind.ItemAdded,
                OrderHistoryItemPayload: null);
            await _orderHistoryRepository.Create(history, ct);
            transaction.Complete();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add item to order {OrderId}", oi.OrderId);
            return false;
        }
    }

    public async Task<bool> RemoveItem(long oId, long oiId, CancellationToken ct)
    {
        using TransactionScope transaction = InitBaseTransaction();

        try
        {
            Order order = await _orderRepository.GetById(oId, ct);

            if (order.OrderState != OrderState.Created)
                return false;

            OrderItem item = await _orderItemRepository.GetById(oiId, ct);

            OrderItem updatedItem = item with { OrderItemDeleted = true };
            await _orderItemRepository.Update(updatedItem, ct);
            var history = new OrderHistory(
                Id: 0,
                OrderId: oId,
                OrderHistoryItemCreatedAt: DateTime.UtcNow,
                OrderHistoryItemKind: OrderHistoryItemKind.ItemRemoved,
                OrderHistoryItemPayload: null);
            await _orderHistoryRepository.Create(history, ct);
            transaction.Complete();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove item {OrderItemId} from order {OrderId}", oiId, oId);
            return false;
        }
    }

    public async Task<bool> TransferToWork(long id, CancellationToken ct)
    {
        using TransactionScope transaction = InitBaseTransaction();

        try
        {
            Order order = await _orderRepository.GetById(id, ct);

            if (order.OrderState != OrderState.Created)
                return false;

            Order updatedOrder = order with { OrderState = OrderState.Processing };
            await _orderRepository.Update(updatedOrder, ct);
            var history = new OrderHistory(
                Id: 0,
                OrderId: id,
                OrderHistoryItemCreatedAt: DateTime.UtcNow,
                OrderHistoryItemKind: OrderHistoryItemKind.StateChanged,
                OrderHistoryItemPayload: null);
            await _orderHistoryRepository.Create(history, ct);
            transaction.Complete();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to transfer order {OrderId} to work", id);
            return false;
        }
    }

    public async Task<bool> CompleteOrder(long id, CancellationToken ct)
    {
        using TransactionScope transaction = InitBaseTransaction();

        try
        {
            Order order = await _orderRepository.GetById(id, ct);

            if (order.OrderState != OrderState.Processing)
                return false;

            Order updatedOrder = order with { OrderState = OrderState.Completed };
            await _orderRepository.Update(updatedOrder, ct);
            var history = new OrderHistory(
                Id: 0,
                OrderId: id,
                OrderHistoryItemCreatedAt: DateTime.UtcNow,
                OrderHistoryItemKind: OrderHistoryItemKind.StateChanged,
                OrderHistoryItemPayload: null);
            await _orderHistoryRepository.Create(history, ct);
            transaction.Complete();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to complete order {OrderId}", id);
            return false;
        }
    }

    public async Task<bool> Cancel(long id, CancellationToken ct)
    {
        using TransactionScope transaction = InitBaseTransaction();

        try
        {
            Order order = await _orderRepository.GetById(id, ct);

            switch (order.OrderState)
            {
                case OrderState.Completed:
                case OrderState.Cancelled:
                    return false;
            }

            Order updatedOrder = order with { OrderState = OrderState.Cancelled };
            await _orderRepository.Update(updatedOrder, ct);
            var history = new OrderHistory(
                Id: 0,
                OrderId: id,
                OrderHistoryItemCreatedAt: DateTime.UtcNow,
                OrderHistoryItemKind: OrderHistoryItemKind.StateChanged,
                OrderHistoryItemPayload: null);
            await _orderHistoryRepository.Create(history, ct);
            transaction.Complete();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel order {OrderId}", id);
            return false;
        }
    }

    public IAsyncEnumerable<OrderHistory> GetHistoryByFilter(
        int position,
        int volume,
        OrderHistoryRequestFiltered request,
        CancellationToken ct)
    {
        return _orderHistoryRepository.GetFiltered(position, volume, request, ct);
    }

    private static TransactionScope InitBaseTransaction()
    {
        return new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);
    }
}