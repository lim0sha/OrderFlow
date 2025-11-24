namespace Gateway.Models.DTO.Orders;

public sealed record RemoveItemRequestDto
{
    public long OrderId { get; init; }

    public long OrderItemId { get; init; }
}