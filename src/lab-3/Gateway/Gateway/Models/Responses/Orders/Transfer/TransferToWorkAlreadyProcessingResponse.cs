namespace Gateway.Models.Responses.Orders.Transfer;

public sealed record TransferToWorkAlreadyProcessingResponse(string Message = "Order is already being processed.")
    : TransferToWorkResponseBase;