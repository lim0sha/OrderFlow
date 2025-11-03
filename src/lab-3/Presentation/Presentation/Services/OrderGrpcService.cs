using DataAccess.Services.Interfaces;
using Grpc.Core;
using Presentation.Protos;
using System.Globalization;
using Order = DataAccess.Models.Entities.Orders.Order;
using OrderHistory = DataAccess.Models.Entities.Orders.OrderHistory;
using OrderHistoryItemKind = DataAccess.Models.Entities.Orders.OrderHistoryItemKind;
using OrderHistoryRequestFiltered = DataAccess.Models.Requests.OrderHistoryRequestFiltered;
using OrderItem = DataAccess.Models.Entities.Orders.OrderItem;
using OrderState = DataAccess.Models.Enums.OrderState;

namespace Presentation.Services;

public class OrderGrpcService : OrderService.OrderServiceBase
{
    private readonly IOrderService _orderService;

    public OrderGrpcService(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public override async Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context)
    {
        var order = new Order(0, OrderState.Created, DateTime.UtcNow, request.CreatedBy);
        bool result = await _orderService.Create(order, context.CancellationToken);

        return result ? new CreateOrderResponse { Success = new CreateOrderSuccess() } : new CreateOrderResponse { Error = new OrderIsNotCreated { Message = "Failed to create order" } };
    }

    public override async Task<AddItemResponse> AddItem(AddItemRequest request, ServerCallContext context)
    {
        var item = new OrderItem
        {
            OrderId = request.OrderId,
            ProductId = request.ProductId,
            OrderItemQuantity = request.Quantity,
            OrderItemDeleted = request.Deleted,
        };

        bool result = await _orderService.AddItem(item, context.CancellationToken);

        return result
            ? new AddItemResponse { Success = new AddItemSuccess { OrderItemId = item.Id } }
            : new AddItemResponse { OrderIsNotCreated = new OrderIsNotCreated { Message = "Failed to add item" } };
    }

    public override async Task<RemoveItemResponse> RemoveItem(RemoveItemRequest request, ServerCallContext context)
    {
        bool result = await _orderService.RemoveItem(request.OrderId, request.OrderItemId, context.CancellationToken);

        return result
            ? new RemoveItemResponse { Success = new RemoveItemSuccess() }
            : new RemoveItemResponse { OrderIsNotCreated = new OrderIsNotCreated { Message = "Failed to remove item" } };
    }

    public override async Task<TransferToWorkResponse> TransferToWork(TransferToWorkRequest request, ServerCallContext context)
    {
        bool result = await _orderService.TransferToWork(request.OrderId, context.CancellationToken);

        return result
            ? new TransferToWorkResponse { Success = new TransferToWorkSuccess() }
            : new TransferToWorkResponse { OrderIsNotCreated = new OrderIsNotCreated { Message = "Failed to transfer to work" } };
    }

    public override async Task<CompleteOrderResponse> CompleteOrder(CompleteOrderRequest request, ServerCallContext context)
    {
        bool result = await _orderService.CompleteOrder(request.OrderId, context.CancellationToken);

        return result
            ? new CompleteOrderResponse { Success = new CompleteOrderSuccess() }
            : new CompleteOrderResponse { OrderNotFound = new OrderNotFound { Message = "Failed to complete order" } };
    }

    public override async Task<CancelOrderResponse> Cancel(CancelOrderRequest request, ServerCallContext context)
    {
        bool result = await _orderService.Cancel(request.OrderId, context.CancellationToken);

        return result
            ? new CancelOrderResponse { Success = new CancelOrderSuccess() }
            : new CancelOrderResponse { OrderNotFound = new OrderNotFound { Message = "Failed to cancel order" } };
    }

    public override async Task<GetHistoryResponse> GetHistoryByFilter(GetHistoryRequest request, ServerCallContext context)
    {
        IAsyncEnumerable<OrderHistory> history = _orderService.GetHistoryByFilter(
            request.Cursor,
            request.Volume,
            new OrderHistoryRequestFiltered
            {
                Id = request.Filter.OrderId,
                OrderHistoryItemKind = (OrderHistoryItemKind)request.Filter.Kind,
            },
            context.CancellationToken);

        var grpcResponse = new GetHistoryResponse();

        await foreach (OrderHistory h in history)
        {
            grpcResponse.History.Add(new Protos.OrderHistory()
            {
                Id = h.Id,
                OrderId = h.OrderId,
                CreatedAt = h.OrderHistoryItemCreatedAt.ToString(CultureInfo.InvariantCulture),
                Kind = (Protos.OrderHistoryItemKind)h.OrderHistoryItemKind,
                Payload = h.OrderHistoryItemPayload?.ToString(),
            });
        }

        return grpcResponse;
    }
}