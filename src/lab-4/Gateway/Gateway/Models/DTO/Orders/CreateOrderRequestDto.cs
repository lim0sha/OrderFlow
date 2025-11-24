namespace Gateway.Models.DTO.Orders;

public sealed record CreateOrderRequestDto
{
    public required string CreatedBy { get; init; }
}
