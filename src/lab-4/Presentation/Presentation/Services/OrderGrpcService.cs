using DataAccess.Repositories.Interfaces;
using DataAccess.Services.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Presentation.Kafka.Abstractions.Interfaces;
using Presentation.Protos;
using System.Globalization;
using Order = DataAccess.Models.Entities.Orders.Order;
using OrderHistory = DataAccess.Models.Entities.Orders.OrderHistory;
using OrderHistoryItemKind = DataAccess.Models.Entities.Orders.OrderHistoryItemKind;
using OrderHistoryRequestFiltered = DataAccess.Models.Requests.OrderHistoryRequestFiltered;
using OrderItem = DataAccess.Models.Entities.Orders.OrderItem;
using OrderState = DataAccess.Models.Enums;

namespace Presentation.Services;

public class OrderGrpcService : OrderService.OrderServiceBase
{
    private readonly IOrderService _orderService;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderHistoryRepository _orderHistoryRepository;
    private readonly IKafkaProducer<long, Orders.Kafka.Contracts.OrderCreationValue> _kafkaProducer;

    public OrderGrpcService(
        IOrderService orderService,
        IOrderRepository orderRepository,
        IKafkaProducer<long, Orders.Kafka.Contracts.OrderCreationValue> kafkaProducer,
        IOrderHistoryRepository orderHistoryRepository)
    {
        _orderService = orderService;
        _orderRepository = orderRepository;
        _kafkaProducer = kafkaProducer;
        _orderHistoryRepository = orderHistoryRepository;
    }

    public override async Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context)
    {
        var order = new Order(0, OrderState.OrderState.Created, DateTime.UtcNow, request.CreatedBy);
        long orderId = await _orderRepository.Create(order, context.CancellationToken);

        if (orderId == 0)
        {
            return new CreateOrderResponse { Error = new OrderIsNotCreated { Message = "Failed to create order" } };
        }

        var history = new OrderHistory(
            Id: 0,
            OrderId: orderId,
            OrderHistoryItemCreatedAt: DateTime.UtcNow,
            OrderHistoryItemKind: OrderHistoryItemKind.Created,
            OrderHistoryItemPayload: null);
        await _orderHistoryRepository.Create(history, context.CancellationToken);

        await _kafkaProducer.ProduceAsync(
            orderId,
            new Orders.Kafka.Contracts.OrderCreationValue
            {
                OrderCreated = new()
                {
                    OrderId = orderId,
                    CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
                },
            },
            context.CancellationToken);

        return new CreateOrderResponse { Success = new CreateOrderSuccess() };
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
            : new RemoveItemResponse
                { OrderIsNotCreated = new OrderIsNotCreated { Message = "Failed to remove item" } };
    }

    public override async Task<TransferToWorkResponse> TransferToWork(
        TransferToWorkRequest request,
        ServerCallContext context)
    {
        bool result = await _orderService.TransferToWork(request.OrderId, context.CancellationToken);

        if (result)
        {
            await _kafkaProducer.ProduceAsync(
                request.OrderId,
                new Orders.Kafka.Contracts.OrderCreationValue
                {
                    OrderProcessingStarted = new()
                    {
                        OrderId = request.OrderId,
                        StartedAt = Timestamp.FromDateTime(DateTime.UtcNow),
                    },
                },
                context.CancellationToken);

            return new TransferToWorkResponse { Success = new TransferToWorkSuccess() };
        }

        return new TransferToWorkResponse
            { OrderIsNotCreated = new OrderIsNotCreated { Message = "Failed to transfer to work" } };
    }

    public override async Task<CancelOrderResponse> Cancel(CancelOrderRequest request, ServerCallContext context)
    {
        Order order = await _orderRepository.GetById(request.OrderId, context.CancellationToken);
        if (order.Id == 0)
            return new CancelOrderResponse { OrderNotFound = new OrderNotFound { Message = "Order not found" } };

        if (order.OrderState != OrderState.OrderState.Created)
        {
            return new CancelOrderResponse
            {
                AlreadyCompleted = new OrderAlreadyCompleted { Message = "Order is not in 'Created' state" },
            };
        }

        bool result = await _orderService.Cancel(request.OrderId, context.CancellationToken);

        return result
            ? new CancelOrderResponse { Success = new CancelOrderSuccess() }
            : new CancelOrderResponse { OrderNotFound = new OrderNotFound { Message = "Failed to cancel order" } };
    }

    public override async Task<GetHistoryResponse> GetHistoryByFilter(
        GetHistoryRequest request,
        ServerCallContext context)
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