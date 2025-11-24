namespace Gateway.Models.Responses.Orders.Create;

public sealed record CreateOrderSuccessResponse(string Message = "Order successfully created.")
    : CreateOrderResponseBase;