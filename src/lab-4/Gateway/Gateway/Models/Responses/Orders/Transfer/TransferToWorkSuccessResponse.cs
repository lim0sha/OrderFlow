namespace Gateway.Models.Responses.Orders.Transfer;

public sealed record TransferToWorkSuccessResponse(string Message = "Order successfully transferred to work.")
    : TransferToWorkResponseBase;