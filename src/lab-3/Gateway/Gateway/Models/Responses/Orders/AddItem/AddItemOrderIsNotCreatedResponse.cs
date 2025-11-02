namespace Gateway.Models.Responses.Orders.AddItem;

public sealed record AddItemOrderIsNotCreatedResponse(string Message = "Order has not been created yet.")
    : AddItemResponseBase;