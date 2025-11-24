namespace Gateway.Models.Responses.Orders.Create;

public sealed record OrderNotFoundResponse(string Message = "Order is not found.") : CreateOrderResponseBase;