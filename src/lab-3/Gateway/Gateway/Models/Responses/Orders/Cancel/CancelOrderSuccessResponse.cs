namespace Gateway.Models.Responses.Orders.Cancel;

public sealed record CancelOrderSuccessResponse(string Message = "Order successfully cancelled.")
    : CancelOrderResponseBase;