using DataAccess.Models.Entities.Common.ResultTypes;
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
        OrderOperationResult result = await _orderService.Create(order, context.CancellationToken);

        return result switch
        {
            OrderOperationResult.Success => new CreateOrderResponse { Success = new CreateOrderSuccess() },
            OrderOperationResult.OrderIsNotCreated e => new CreateOrderResponse { Error = new OrderIsNotCreated { Message = e.ErrorMessage } },
            OrderOperationResult.OrderNotFound e => new CreateOrderResponse { NotFound = new OrderNotFound { Message = e.ErrorMessage } },
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown error")),
        };
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

        OrderOperationResult result = await _orderService.AddItem(item, context.CancellationToken);

        return result switch
        {
            OrderOperationResult.Success success => new AddItemResponse
            {
                Success = new AddItemSuccess { OrderItemId = item.Id },
            },
            OrderOperationResult.OrderIsNotCreated e => new AddItemResponse
            {
                OrderIsNotCreated = new OrderIsNotCreated { Message = e.ErrorMessage },
            },
            OrderOperationResult.OrderNotFound e => new AddItemResponse
            {
                OrderNotFound = new OrderNotFound { Message = e.ErrorMessage },
            },
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown error")),
        };
    }

    public override async Task<RemoveItemResponse> RemoveItem(RemoveItemRequest request, ServerCallContext context)
    {
        OrderOperationResult result = await _orderService.RemoveItem(request.OrderId, request.OrderItemId, context.CancellationToken);

        return result switch
        {
            OrderOperationResult.Success => new RemoveItemResponse { Success = new RemoveItemSuccess() },
            OrderOperationResult.OrderIsNotCreated e => new RemoveItemResponse { OrderIsNotCreated = new OrderIsNotCreated { Message = e.ErrorMessage } },
            OrderOperationResult.OrderNotFound e => new RemoveItemResponse { OrderNotFound = new OrderNotFound { Message = e.ErrorMessage } },
            OrderOperationResult.OrderItemNotFound e => new RemoveItemResponse { OrderItemNotFound = new OrderItemNotFound { Message = e.ErrorMessage } },
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown error")),
        };
    }

    public override async Task<TransferToWorkResponse> TransferToWork(TransferToWorkRequest request, ServerCallContext context)
    {
        OrderOperationResult result = await _orderService.TransferToWork(request.OrderId, context.CancellationToken);

        return result switch
        {
            OrderOperationResult.Success => new TransferToWorkResponse { Success = new TransferToWorkSuccess() },
            OrderOperationResult.OrderIsNotCreated e => new TransferToWorkResponse { OrderIsNotCreated = new OrderIsNotCreated { Message = e.ErrorMessage } },
            OrderOperationResult.OrderNotFound e => new TransferToWorkResponse { OrderNotFound = new OrderNotFound { Message = e.ErrorMessage } },
            OrderOperationResult.OrderAlreadyProcessing e => new TransferToWorkResponse { AlreadyProcessing = new OrderAlreadyProcessing { Message = e.ErrorMessage } },
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown error")),
        };
    }

    public override async Task<CompleteOrderResponse> CompleteOrder(CompleteOrderRequest request, ServerCallContext context)
    {
        OrderOperationResult result = await _orderService.CompleteOrder(request.OrderId, context.CancellationToken);

        return result switch
        {
            OrderOperationResult.Success => new CompleteOrderResponse { Success = new CompleteOrderSuccess() },
            OrderOperationResult.OrderNotFound e => new CompleteOrderResponse { OrderNotFound = new OrderNotFound { Message = e.ErrorMessage } },
            OrderOperationResult.OrderAlreadyCompleted e => new CompleteOrderResponse { AlreadyCompleted = new OrderAlreadyCompleted { Message = e.ErrorMessage } },
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown error")),
        };
    }

    public override async Task<CancelOrderResponse> Cancel(CancelOrderRequest request, ServerCallContext context)
    {
        OrderOperationResult result = await _orderService.Cancel(request.OrderId, context.CancellationToken);

        return result switch
        {
            OrderOperationResult.Success => new CancelOrderResponse { Success = new CancelOrderSuccess() },
            OrderOperationResult.OrderNotFound e => new CancelOrderResponse { OrderNotFound = new OrderNotFound { Message = e.ErrorMessage } },
            OrderOperationResult.OrderAlreadyCancelled e => new CancelOrderResponse { AlreadyCancelled = new OrderAlreadyCancelled { Message = e.ErrorMessage } },
            OrderOperationResult.OrderAlreadyCompleted e => new CancelOrderResponse { AlreadyCompleted = new OrderAlreadyCompleted { Message = e.ErrorMessage } },
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown error")),
        };
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
