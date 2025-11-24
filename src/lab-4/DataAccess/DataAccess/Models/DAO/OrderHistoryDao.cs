using DataAccess.Models.Entities.Orders;

namespace DataAccess.Models.DAO;

public record OrderHistoryDao(
    long OrderHistoryItemId,
    long OrderId,
    DateTime OrderHistoryItemCreatedAt,
    OrderHistoryItemKind OrderHistoryItemKind,
    string? OrderHistoryItemPayload);