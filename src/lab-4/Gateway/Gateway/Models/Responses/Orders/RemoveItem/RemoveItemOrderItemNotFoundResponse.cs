namespace Gateway.Models.Responses.Orders.RemoveItem;

public sealed record RemoveItemOrderItemNotFoundResponse(string Message = "Order item not found.")
    : RemoveItemResponseBase;