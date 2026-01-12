namespace Gateway.Models.Responses.Orders.Transfer;

public sealed record TransferToWorkOrderIsNotCreatedResponse(string Message = "Order is not created.") : TransferToWorkResponseBase;