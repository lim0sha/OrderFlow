namespace Gateway.Models.Responses.Orders.AddItem;

public sealed record AddItemOrderNotFoundResponse(string Message = "Order not found.")
    : AddItemResponseBase;