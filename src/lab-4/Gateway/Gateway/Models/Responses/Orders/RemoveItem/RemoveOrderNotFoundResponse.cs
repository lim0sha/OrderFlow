namespace Gateway.Models.Responses.Orders.RemoveItem;

public sealed record RemoveOrderNotFoundResponse(string Message = "Order not found") : RemoveItemResponseBase;