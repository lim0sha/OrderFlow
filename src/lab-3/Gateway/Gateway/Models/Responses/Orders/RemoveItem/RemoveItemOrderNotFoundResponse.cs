namespace Gateway.Models.Responses.Orders.RemoveItem;

public sealed record RemoveItemOrderNotFoundResponse(string Message = "Order not found.")
    : RemoveItemResponseBase;