namespace Gateway.Models.Responses.Orders.Cancel;

public sealed record CancelOrderAlreadyCancelledResponse(string Message = "Order has already been cancelled.")
    : CancelOrderResponseBase;