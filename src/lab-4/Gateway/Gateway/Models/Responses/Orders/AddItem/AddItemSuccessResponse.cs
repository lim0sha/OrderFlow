namespace Gateway.Models.Responses.Orders.AddItem;

public sealed record AddItemSuccessResponse(long OrderItemId, string Message = "Item successfully added to the order.")
    : AddItemResponseBase;