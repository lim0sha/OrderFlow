namespace Gateway.Models.Responses.Orders.RemoveItem;

public sealed record RemoveItemSuccessResponse(string Message = "Item removed successfully") : RemoveItemResponseBase;
