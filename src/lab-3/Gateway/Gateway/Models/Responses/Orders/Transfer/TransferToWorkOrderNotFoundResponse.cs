namespace Gateway.Models.Responses.Orders.Transfer;

public sealed record TransferToWorkOrderNotFoundResponse(string Message = "Order not found.") : TransferToWorkResponseBase;