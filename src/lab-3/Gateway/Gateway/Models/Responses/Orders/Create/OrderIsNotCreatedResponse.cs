namespace Gateway.Models.Responses.Orders.Create;

public sealed record OrderIsNotCreatedResponse(string Message = "Order is not created.") : CreateOrderResponseBase;