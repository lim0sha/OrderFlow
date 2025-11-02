using Gateway.Models.DTO.Orders;
using Gateway.Models.Responses.Orders.AddItem;
using Gateway.Models.Responses.Orders.Cancel;
using Gateway.Models.Responses.Orders.Complete;
using Gateway.Models.Responses.Orders.Create;
using Gateway.Models.Responses.Orders.RemoveItem;
using Gateway.Models.Responses.Orders.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Protos;
using Swashbuckle.AspNetCore.Annotations;

namespace Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
#pragma warning disable CA1506
public class OrderController : ControllerBase
#pragma warning restore CA1506
{
    private readonly OrderService.OrderServiceClient _orderClient;

    public OrderController(OrderService.OrderServiceClient orderClient)
    {
        _orderClient = orderClient;
    }

    [HttpPost("create")]
    [SwaggerResponse(StatusCodes.Status200OK, "Order created successfully", typeof(CreateOrderSuccessResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Order not created", typeof(OrderIsNotCreatedResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found", typeof(OrderNotFoundResponse))]
    public async Task<ActionResult<CreateOrderResponseBase>> CreateOrder([FromBody] CreateOrderRequestDto request)
    {
        CreateOrderResponse grpcResponse = await _orderClient.CreateOrderAsync(new CreateOrderRequest
        {
            CreatedBy = request.CreatedBy,
        });

        return grpcResponse.ResultCase switch
        {
            CreateOrderResponse.ResultOneofCase.Success =>
                Ok(new CreateOrderSuccessResponse()),

            CreateOrderResponse.ResultOneofCase.Error =>
                BadRequest(new OrderIsNotCreatedResponse(grpcResponse.Error.Message)),

            CreateOrderResponse.ResultOneofCase.NotFound =>
                NotFound(new OrderNotFoundResponse(grpcResponse.NotFound.Message)),

            CreateOrderResponse.ResultOneofCase.None =>
                NotFound(new CreateOrderNoneResponse(grpcResponse.NotFound.Message)),

            _ => StatusCode(StatusCodes.Status500InternalServerError),
        };
    }

    [HttpPost("add-item")]
    [SwaggerResponse(StatusCodes.Status200OK, "Item added successfully", typeof(AddItemSuccessResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Order is not created", typeof(AddItemOrderIsNotCreatedResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found", typeof(AddItemOrderNotFoundResponse))]
    public async Task<ActionResult<AddItemResponseBase>> AddItem([FromBody] AddItemRequestDto request)
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
            AddItemResponse.ResultOneofCase.Success =>
                Ok(new AddItemSuccessResponse(grpcResponse.Success.OrderItemId)),

            AddItemResponse.ResultOneofCase.OrderIsNotCreated =>
                BadRequest(new AddItemOrderIsNotCreatedResponse(grpcResponse.OrderIsNotCreated.Message)),

            AddItemResponse.ResultOneofCase.OrderNotFound =>
                NotFound(new AddItemOrderNotFoundResponse(grpcResponse.OrderNotFound.Message)),

            AddItemResponse.ResultOneofCase.None =>
                StatusCode(StatusCodes.Status500InternalServerError),

            _ => StatusCode(500),
        };
    }

    [HttpPost("remove-item")]
    [SwaggerResponse(StatusCodes.Status200OK, "Item removed successfully", typeof(RemoveItemSuccessResponse))]
    [SwaggerResponse(
        StatusCodes.Status404NotFound,
        "Order item not found",
        typeof(RemoveItemOrderItemNotFoundResponse))]
    public async Task<ActionResult<RemoveItemResponseBase>> RemoveItem([FromBody] RemoveItemRequestDto request)
    {
        RemoveItemResponse grpcResponse = await _orderClient.RemoveItemAsync(new RemoveItemRequest
        {
            OrderId = request.OrderId,
            OrderItemId = request.OrderItemId,
        });

        return grpcResponse.ResultCase switch
        {
            RemoveItemResponse.ResultOneofCase.Success =>
                Ok(new RemoveItemSuccessResponse()),

            RemoveItemResponse.ResultOneofCase.OrderItemNotFound =>
                NotFound(new RemoveItemOrderItemNotFoundResponse(grpcResponse.OrderItemNotFound.Message)),

            RemoveItemResponse.ResultOneofCase.None =>
                StatusCode(StatusCodes.Status500InternalServerError),

            RemoveItemResponse.ResultOneofCase.OrderIsNotCreated =>
                NotFound(new RemoveItemOrderIsNotCreated(grpcResponse.OrderIsNotCreated.Message)),

            RemoveItemResponse.ResultOneofCase.OrderNotFound =>
                NotFound(new RemoveOrderNotFoundResponse(grpcResponse.OrderItemNotFound.Message)),

            _ => BadRequest(),
        };
    }

    [HttpPost("transfer-to-work/{orderId:long}")]
    [SwaggerResponse(StatusCodes.Status200OK, "Order transferred to work", typeof(TransferToWorkSuccessResponse))]
    [SwaggerResponse(
        StatusCodes.Status409Conflict,
        "Already processing",
        typeof(TransferToWorkAlreadyProcessingResponse))]
    public async Task<ActionResult<TransferToWorkResponseBase>> TransferToWork(long orderId)
    {
        TransferToWorkResponse grpcResponse =
            await _orderClient.TransferToWorkAsync(new TransferToWorkRequest { OrderId = orderId });

        return grpcResponse.ResultCase switch
        {
            TransferToWorkResponse.ResultOneofCase.Success =>
                Ok(new TransferToWorkSuccessResponse()),

            TransferToWorkResponse.ResultOneofCase.AlreadyProcessing =>
                Conflict(new TransferToWorkAlreadyProcessingResponse(grpcResponse.AlreadyProcessing.Message)),

            TransferToWorkResponse.ResultOneofCase.None =>
                NotFound(new TransferToWorkNoneResponse(grpcResponse.OrderNotFound.Message)),

            TransferToWorkResponse.ResultOneofCase.OrderIsNotCreated =>
                NotFound(new TransferToWorkOrderIsNotCreatedResponse(grpcResponse.OrderIsNotCreated.Message)),

            TransferToWorkResponse.ResultOneofCase.OrderNotFound =>
                NotFound(new TransferToWorkOrderNotFoundResponse(grpcResponse.OrderNotFound.Message)),

            _ => BadRequest(),
        };
    }

    [HttpPost("complete/{orderId:long}")]
    [SwaggerResponse(StatusCodes.Status200OK, "Order completed", typeof(CompleteOrderSuccessResponse))]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Already completed", typeof(CompleteOrderAlreadyCompletedResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found", typeof(CompleteOrderNotFoundResponse))]
    public async Task<ActionResult<CompleteOrderResponseBase>> Complete(long orderId)
    {
        CompleteOrderResponse grpcResponse =
            await _orderClient.CompleteOrderAsync(new CompleteOrderRequest { OrderId = orderId });

        return grpcResponse.ResultCase switch
        {
            CompleteOrderResponse.ResultOneofCase.Success =>
                Ok(new CompleteOrderSuccessResponse()),

            CompleteOrderResponse.ResultOneofCase.AlreadyCompleted =>
                Conflict(new CompleteOrderAlreadyCompletedResponse(grpcResponse.AlreadyCompleted.Message)),

            CompleteOrderResponse.ResultOneofCase.OrderNotFound =>
                NotFound(new CompleteOrderNotFoundResponse(grpcResponse.OrderNotFound.Message)),

            CompleteOrderResponse.ResultOneofCase.None =>
                NotFound(new CompleteOrderNoneResponse(grpcResponse.OrderNotFound.Message)),

            _ => StatusCode(500),
        };
    }

    [HttpPost("cancel/{orderId:long}")]
    [SwaggerResponse(StatusCodes.Status200OK, "Order cancelled", typeof(CancelOrderSuccessResponse))]
    [SwaggerResponse(
        StatusCodes.Status409Conflict,
        "Order already cancelled",
        typeof(CancelOrderAlreadyCancelledResponse))]
    public async Task<ActionResult<CancelOrderResponseBase>> Cancel(long orderId)
    {
        CancelOrderResponse grpcResponse = await _orderClient.CancelAsync(new CancelOrderRequest { OrderId = orderId });

        return grpcResponse.ResultCase switch
        {
            CancelOrderResponse.ResultOneofCase.Success =>
                Ok(new CancelOrderSuccessResponse()),

            CancelOrderResponse.ResultOneofCase.AlreadyCancelled =>
                Conflict(new CancelOrderAlreadyCancelledResponse(grpcResponse.AlreadyCancelled.Message)),
            CancelOrderResponse.ResultOneofCase.None =>
                NotFound(new CancelOrderNoneResponse(grpcResponse.OrderNotFound.Message)),

            CancelOrderResponse.ResultOneofCase.OrderNotFound =>
                NotFound(new CancelOrderOrderNotFoundResponse(grpcResponse.OrderNotFound.Message)),

            CancelOrderResponse.ResultOneofCase.AlreadyCompleted =>
                NotFound(new CancelOrderAlreadyCompletedResponse(grpcResponse.OrderNotFound.Message)),

            _ => BadRequest(),
        };
    }

    [HttpGet("history")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns order history")]
    public async Task<IActionResult> GetHistory([FromQuery] GetHistoryRequestDto request)
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

        IEnumerable<OrderHistoryDto> history = grpcResponse.History.Select(h => new OrderHistoryDto
        {
            Id = h.Id,
            OrderId = h.OrderId,
            CreatedAt = h.CreatedAt,
            Kind = h.Kind.ToString(),
            Payload = h.Payload,
        });

        return Ok(history);
    }
}