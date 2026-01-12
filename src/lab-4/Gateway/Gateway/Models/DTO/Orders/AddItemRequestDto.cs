namespace Gateway.Models.DTO.Orders;

public sealed record AddItemRequestDto
{
    public long OrderId { get; init; }

    public long ProductId { get; init; }

    public int Quantity { get; init; }

    public bool Deleted { get; init; }
}