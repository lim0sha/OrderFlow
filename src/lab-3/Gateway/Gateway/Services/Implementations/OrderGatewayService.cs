using Gateway.Models.DTO.Orders;
using Gateway.Models.Responses.Orders.AddItem;
using Gateway.Models.Responses.Orders.Cancel;
using Gateway.Models.Responses.Orders.Complete;
using Gateway.Models.Responses.Orders.Create;
using Gateway.Models.Responses.Orders.RemoveItem;
using Gateway.Models.Responses.Orders.Transfer;
using Gateway.Services.Interfaces;
using Presentation.Protos;

namespace Gateway.Services.Implementations;

public class OrderGatewayService : IOrderGatewayService
{
    private readonly OrderService.OrderServiceClient _orderClient;

    public OrderGatewayService(OrderService.OrderServiceClient orderClient)
    {
        _orderClient = orderClient;
    }

    public async Task<CreateOrderResponseBase> CreateOrderAsync(CreateOrderRequestDto request)
    {
        CreateOrderResponse grpcResponse = await _orderClient.CreateOrderAsync(new CreateOrderRequest
        {
            CreatedBy = request.CreatedBy,
        });

        return grpcResponse.ResultCase switch
        {
            CreateOrderResponse.ResultOneofCase.Success => new CreateOrderSuccessResponse(),
            CreateOrderResponse.ResultOneofCase.Error => new OrderIsNotCreatedResponse(grpcResponse.Error.Message),
            CreateOrderResponse.ResultOneofCase.NotFound => new OrderNotFoundResponse(grpcResponse.NotFound.Message),
            CreateOrderResponse.ResultOneofCase.None => new CreateOrderNoneResponse("Unknown error"),
            _ => throw new InvalidOperationException("Unexpected gRPC response case"),
        };
    }

    public async Task<AddItemResponseBase> AddItemAsync(AddItemRequestDto request)
    {
        AddItemResponse grpcResponse = await _orderClient.AddItemAsync(new AddItemRequest
        {
            OrderId = request.OrderId,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            Deleted = request.Deleted,
        });

        return grpcResponse.ResultCase switch
        {
            AddItemResponse.ResultOneofCase.Success => new AddItemSuccessResponse(grpcResponse.Success.OrderItemId),
            AddItemResponse.ResultOneofCase.OrderIsNotCreated =>
                new AddItemOrderIsNotCreatedResponse(grpcResponse.OrderIsNotCreated.Message),
            AddItemResponse.ResultOneofCase.OrderNotFound =>
                new AddItemOrderNotFoundResponse(grpcResponse.OrderNotFound.Message),
            AddItemResponse.ResultOneofCase.None => throw new NotImplementedException(),
            _ => throw new InvalidOperationException("Unexpected gRPC response case"),
        };
    }

    public async Task<RemoveItemResponseBase> RemoveItemAsync(RemoveItemRequestDto request)
    {
        RemoveItemResponse grpcResponse = await _orderClient.RemoveItemAsync(new RemoveItemRequest
        {
            OrderId = request.OrderId,
            OrderItemId = request.OrderItemId,
        });

        return grpcResponse.ResultCase switch
        {
            RemoveItemResponse.ResultOneofCase.Success => new RemoveItemSuccessResponse(),
            RemoveItemResponse.ResultOneofCase.OrderItemNotFound =>
                new RemoveItemOrderItemNotFoundResponse(grpcResponse.OrderItemNotFound.Message),
            RemoveItemResponse.ResultOneofCase.OrderIsNotCreated =>
                new RemoveItemOrderIsNotCreated(grpcResponse.OrderIsNotCreated.Message),
            RemoveItemResponse.ResultOneofCase.OrderNotFound =>
                new RemoveOrderNotFoundResponse(grpcResponse.OrderNotFound.Message),
            RemoveItemResponse.ResultOneofCase.None => throw new NotImplementedException(),
            _ => throw new InvalidOperationException("Unexpected gRPC response case"),
        };
    }

    public async Task<TransferToWorkResponseBase> TransferToWorkAsync(long orderId)
    {
        TransferToWorkResponse grpcResponse = await _orderClient.TransferToWorkAsync(new TransferToWorkRequest { OrderId = orderId });

        return grpcResponse.ResultCase switch
        {
            TransferToWorkResponse.ResultOneofCase.Success => new TransferToWorkSuccessResponse(),
            TransferToWorkResponse.ResultOneofCase.AlreadyProcessing =>
                new TransferToWorkAlreadyProcessingResponse(grpcResponse.AlreadyProcessing.Message),
            TransferToWorkResponse.ResultOneofCase.OrderIsNotCreated =>
                new TransferToWorkOrderIsNotCreatedResponse(grpcResponse.OrderIsNotCreated.Message),
            TransferToWorkResponse.ResultOneofCase.OrderNotFound =>
                new TransferToWorkOrderNotFoundResponse(grpcResponse.OrderNotFound.Message),
            TransferToWorkResponse.ResultOneofCase.None => throw new NotImplementedException(),
            _ => throw new InvalidOperationException("Unexpected gRPC response case"),
        };
    }

    public async Task<CompleteOrderResponseBase> CompleteOrderAsync(long orderId)
    {
        CompleteOrderResponse grpcResponse = await _orderClient.CompleteOrderAsync(new CompleteOrderRequest { OrderId = orderId });

        return grpcResponse.ResultCase switch
        {
            CompleteOrderResponse.ResultOneofCase.Success => new CompleteOrderSuccessResponse(),
            CompleteOrderResponse.ResultOneofCase.AlreadyCompleted =>
                new CompleteOrderAlreadyCompletedResponse(grpcResponse.AlreadyCompleted.Message),
            CompleteOrderResponse.ResultOneofCase.OrderNotFound =>
                new CompleteOrderNotFoundResponse(grpcResponse.OrderNotFound.Message),
            CompleteOrderResponse.ResultOneofCase.None => throw new NotImplementedException(),
            _ => throw new InvalidOperationException("Unexpected gRPC response case"),
        };
    }

    public async Task<CancelOrderResponseBase> CancelOrderAsync(long orderId)
    {
        CancelOrderResponse grpcResponse = await _orderClient.CancelAsync(new CancelOrderRequest { OrderId = orderId });

        return grpcResponse.ResultCase switch
        {
            CancelOrderResponse.ResultOneofCase.Success => new CancelOrderSuccessResponse(),
            CancelOrderResponse.ResultOneofCase.AlreadyCancelled =>
                new CancelOrderAlreadyCancelledResponse(grpcResponse.AlreadyCancelled.Message),
            CancelOrderResponse.ResultOneofCase.AlreadyCompleted =>
                new CancelOrderAlreadyCompletedResponse(grpcResponse.AlreadyCompleted.Message),
            CancelOrderResponse.ResultOneofCase.OrderNotFound =>
                new CancelOrderOrderNotFoundResponse(grpcResponse.OrderNotFound.Message),
            CancelOrderResponse.ResultOneofCase.None => throw new NotImplementedException(),
            _ => throw new InvalidOperationException("Unexpected gRPC response case"),
        };
    }

    public async Task<IEnumerable<OrderHistoryDto>> GetHistoryAsync(GetHistoryRequestDto request)
    {
        GetHistoryResponse grpcResponse = await _orderClient.GetHistoryByFilterAsync(new GetHistoryRequest
        {
            Cursor = request.Cursor,
            Volume = request.Volume,
            Filter = new OrderHistoryRequestFiltered
            {
                OrderId = request.Filter.OrderId,
                Kind = (OrderHistoryItemKind)request.Filter.Kind,
            },
        });

        return grpcResponse.History.Select(h => new OrderHistoryDto
        {
            Id = h.Id,
            OrderId = h.OrderId,
            CreatedAt = h.CreatedAt,
            Kind = h.Kind.ToString(),
            Payload = h.Payload,
        });
    }
}