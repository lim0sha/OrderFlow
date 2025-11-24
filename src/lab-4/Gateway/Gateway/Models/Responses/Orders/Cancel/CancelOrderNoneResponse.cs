namespace Gateway.Models.Responses.Orders.Cancel;

public sealed record CancelOrderNoneResponse(string Message = "None") : CancelOrderResponseBase;