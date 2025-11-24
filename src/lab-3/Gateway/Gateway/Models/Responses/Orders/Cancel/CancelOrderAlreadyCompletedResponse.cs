namespace Gateway.Models.Responses.Orders.Cancel;

public sealed record CancelOrderAlreadyCompletedResponse(string Message = "Already completed") : CancelOrderResponseBase;