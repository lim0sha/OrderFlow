using Gateway.Models.DTO.Orders;
using Gateway.Models.Responses.Orders.AddItem;
using Gateway.Models.Responses.Orders.Cancel;
using Gateway.Models.Responses.Orders.Create;
using Gateway.Models.Responses.Orders.RemoveItem;
using Gateway.Models.Responses.Orders.Transfer;
using Gateway.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
#pragma warning disable CA1506
public class OrderController : ControllerBase
#pragma warning restore CA1506
{
    private readonly IOrderGatewayService _orderService;

    public OrderController(IOrderGatewayService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost("create")]
    [SwaggerResponse(StatusCodes.Status200OK, "Order created successfully", typeof(CreateOrderSuccessResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Order not created", typeof(OrderIsNotCreatedResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found", typeof(OrderNotFoundResponse))]
    public async Task<ActionResult<CreateOrderResponseBase>> CreateOrder([FromBody] CreateOrderRequestDto request)
    {
        CreateOrderResponseBase response = await _orderService.CreateOrderAsync(request);
        return response switch
        {
            CreateOrderSuccessResponse => Ok(response),
            OrderIsNotCreatedResponse => BadRequest(response),
            OrderNotFoundResponse => NotFound(response),
            CreateOrderNoneResponse => NotFound(response),
            _ => StatusCode(StatusCodes.Status500InternalServerError),
        };
    }

    [HttpPost("add-item")]
    [SwaggerResponse(StatusCodes.Status200OK, "Item added successfully", typeof(AddItemSuccessResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Order is not created", typeof(AddItemOrderIsNotCreatedResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found", typeof(AddItemOrderNotFoundResponse))]
    public async Task<ActionResult<AddItemResponseBase>> AddItem([FromBody] AddItemRequestDto request)
    {
        AddItemResponseBase response = await _orderService.AddItemAsync(request);
        return response switch
        {
            AddItemSuccessResponse => Ok(response),
            AddItemOrderIsNotCreatedResponse => BadRequest(response),
            AddItemOrderNotFoundResponse => NotFound(response),
            _ => StatusCode(StatusCodes.Status500InternalServerError),
        };
    }

    [HttpPost("remove-item")]
    [SwaggerResponse(StatusCodes.Status200OK, "Item removed successfully", typeof(RemoveItemSuccessResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Order item not found", typeof(RemoveItemOrderItemNotFoundResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found", typeof(RemoveOrderNotFoundResponse))]
    public async Task<ActionResult<RemoveItemResponseBase>> RemoveItem([FromBody] RemoveItemRequestDto request)
    {
        RemoveItemResponseBase response = await _orderService.RemoveItemAsync(request);
        return response switch
        {
            RemoveItemSuccessResponse => Ok(response),
            RemoveItemOrderItemNotFoundResponse => NotFound(response),
            RemoveItemOrderIsNotCreated => NotFound(response),
            RemoveOrderNotFoundResponse => NotFound(response),
            _ => StatusCode(StatusCodes.Status500InternalServerError),
        };
    }

    [HttpPost("transfer-to-work/{orderId:long}")]
    [SwaggerResponse(StatusCodes.Status200OK, "Order transferred to work", typeof(TransferToWorkSuccessResponse))]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Already processing", typeof(TransferToWorkAlreadyProcessingResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found", typeof(TransferToWorkOrderNotFoundResponse))]
    public async Task<ActionResult<TransferToWorkResponseBase>> TransferToWork(long orderId)
    {
        TransferToWorkResponseBase response = await _orderService.TransferToWorkAsync(orderId);
        return response switch
        {
            TransferToWorkSuccessResponse => Ok(response),
            TransferToWorkAlreadyProcessingResponse => Conflict(response),
            TransferToWorkOrderIsNotCreatedResponse => NotFound(response),
            TransferToWorkOrderNotFoundResponse => NotFound(response),
            _ => StatusCode(StatusCodes.Status500InternalServerError),
        };
    }

    [HttpPost("cancel/{orderId:long}")]
    [SwaggerResponse(StatusCodes.Status200OK, "Order cancelled", typeof(CancelOrderSuccessResponse))]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Order already cancelled", typeof(CancelOrderAlreadyCancelledResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found", typeof(CancelOrderOrderNotFoundResponse))]
    public async Task<ActionResult<CancelOrderResponseBase>> Cancel(long orderId)
    {
        CancelOrderResponseBase response = await _orderService.CancelOrderAsync(orderId);
        return response switch
        {
            CancelOrderSuccessResponse => Ok(response),
            CancelOrderAlreadyCancelledResponse => Conflict(response),
            CancelOrderAlreadyCompletedResponse => NotFound(response),
            CancelOrderOrderNotFoundResponse => NotFound(response),
            _ => StatusCode(StatusCodes.Status500InternalServerError),
        };
    }

    [HttpGet("history")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns order history", Type = typeof(IEnumerable<OrderHistoryDto>))]
    public async Task<ActionResult<IEnumerable<OrderHistoryDto>>> GetHistory([FromQuery] GetHistoryRequestDto request)
    {
        IEnumerable<OrderHistoryDto> history = await _orderService.GetHistoryAsync(request);
        return Ok(history);
    }
}