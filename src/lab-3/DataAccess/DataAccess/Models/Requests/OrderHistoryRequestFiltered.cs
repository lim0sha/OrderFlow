using DataAccess.Models.Entities.Orders;

namespace DataAccess.Models.Requests;

public record struct OrderHistoryRequestFiltered(long? Id, OrderHistoryItemKind OrderHistoryItemKind);