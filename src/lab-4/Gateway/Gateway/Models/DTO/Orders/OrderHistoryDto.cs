namespace Gateway.Models.DTO.Orders;

public sealed record OrderHistoryDto
{
    public long Id { get; init; }

    public long OrderId { get; init; }

    public string CreatedAt { get; init; } = string.Empty;

    public string Kind { get; init; } = string.Empty;

    public string Payload { get; init; } = string.Empty;
}