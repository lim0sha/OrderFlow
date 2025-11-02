using DataAccess.Models.Entities.Common.ResultTypes;
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

    public async Task<OrderOperationResult> Create(Order o, CancellationToken ct)
    {
        var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

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
            return new OrderOperationResult.Success(orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create order");
            return new OrderOperationResult.InvalidOperation("Failed to create order");
        }
        finally
        {
            transaction.Dispose();
        }
    }

    public async Task<OrderOperationResult> AddItem(OrderItem oi, CancellationToken ct)
    {
        var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        try
        {
            Order order = await _orderRepository.GetById(oi.OrderId, ct);

            if (order.OrderState != OrderState.Created)
                return new OrderOperationResult.OrderIsNotCreated();

            long orderItemId = await _orderItemRepository.Create(oi, ct);
            var history = new OrderHistory(
                Id: 0,
                OrderId: oi.OrderId,
                OrderHistoryItemCreatedAt: DateTime.UtcNow,
                OrderHistoryItemKind: OrderHistoryItemKind.ItemAdded,
                OrderHistoryItemPayload: null);
            await _orderHistoryRepository.Create(history, ct);
            transaction.Complete();
            return new OrderOperationResult.Success(orderItemId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add item to order {OrderId}", oi.OrderId);
            return new OrderOperationResult.InvalidOperation("Failed to add item to order");
        }
        finally
        {
            transaction.Dispose();
        }
    }

    public async Task<OrderOperationResult> RemoveItem(long oId, long oiId, CancellationToken ct)
    {
        var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        try
        {
            Order order = await _orderRepository.GetById(oId, ct);

            if (order.OrderState != OrderState.Created)
                return new OrderOperationResult.OrderIsNotCreated();

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
            return new OrderOperationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove item {OrderItemId} from order {OrderId}", oiId, oId);
            return new OrderOperationResult.InvalidOperation("Failed to remove item from order");
        }
        finally
        {
            transaction.Dispose();
        }
    }

    public async Task<OrderOperationResult> TransferToWork(long id, CancellationToken ct)
    {
        var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        try
        {
            Order order = await _orderRepository.GetById(id, ct);

            if (order.OrderState != OrderState.Created)
                return new OrderOperationResult.OrderAlreadyProcessing();

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
            return new OrderOperationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to transfer order {OrderId} to work", id);
            return new OrderOperationResult.InvalidOperation("Failed to transfer order to work");
        }
        finally
        {
            transaction.Dispose();
        }
    }

    public async Task<OrderOperationResult> CompleteOrder(long id, CancellationToken ct)
    {
        var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        try
        {
            Order order = await _orderRepository.GetById(id, ct);

            if (order.OrderState != OrderState.Processing)
                return new OrderOperationResult.OrderAlreadyCompleted();

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
            return new OrderOperationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to complete order {OrderId}", id);
            return new OrderOperationResult.InvalidOperation("Failed to complete order");
        }
        finally
        {
            transaction.Dispose();
        }
    }

    public async Task<OrderOperationResult> Cancel(long id, CancellationToken ct)
    {
        var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        try
        {
            Order order = await _orderRepository.GetById(id, ct);

            switch (order.OrderState)
            {
                case OrderState.Completed:
                    return new OrderOperationResult.OrderAlreadyCompleted();
                case OrderState.Cancelled:
                    return new OrderOperationResult.OrderAlreadyCancelled();
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
            return new OrderOperationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel order {OrderId}", id);
            return new OrderOperationResult.InvalidOperation("Failed to cancel order");
        }
        finally
        {
            transaction.Dispose();
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
}
