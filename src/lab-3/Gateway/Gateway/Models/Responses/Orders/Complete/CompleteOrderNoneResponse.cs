namespace Gateway.Models.Responses.Orders.Complete;

public sealed record CompleteOrderNoneResponse(string Message = "None") : CompleteOrderResponseBase;