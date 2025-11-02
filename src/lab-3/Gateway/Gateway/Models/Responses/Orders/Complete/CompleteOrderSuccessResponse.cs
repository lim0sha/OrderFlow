namespace Gateway.Models.Responses.Orders.Complete;

public sealed record CompleteOrderSuccessResponse(string Message = "Order successfully completed.")
    : CompleteOrderResponseBase;
