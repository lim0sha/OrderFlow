namespace Gateway.Models.Responses.Orders.Complete;

public sealed record CompleteOrderNotFoundResponse(string Message = "Order not found.")
    : CompleteOrderResponseBase;