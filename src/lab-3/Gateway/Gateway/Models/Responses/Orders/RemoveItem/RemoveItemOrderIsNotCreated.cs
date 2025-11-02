namespace Gateway.Models.Responses.Orders.RemoveItem;

public sealed record RemoveItemOrderIsNotCreated(string Message = "Order is not created") : RemoveItemResponseBase;