namespace DataAccess.Models.Entities.Orders;

public record OrderHistory(
    long Id,
    long OrderId,
    DateTime OrderHistoryItemCreatedAt,
    OrderHistoryItemKind OrderHistoryItemKind,
    Carry.CarryBase? OrderHistoryItemPayload);