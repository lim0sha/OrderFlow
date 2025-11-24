namespace Gateway.Models.Responses.Orders.Complete;

public sealed record CompleteOrderAlreadyCompletedResponse(string Message = "Order has already been completed.")
    : CompleteOrderResponseBase;