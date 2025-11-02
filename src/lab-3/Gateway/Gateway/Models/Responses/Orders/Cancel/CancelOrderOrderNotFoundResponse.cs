namespace Gateway.Models.Responses.Orders.Cancel;

public sealed record CancelOrderOrderNotFoundResponse(string Message = "Order not found.") : CancelOrderResponseBase;