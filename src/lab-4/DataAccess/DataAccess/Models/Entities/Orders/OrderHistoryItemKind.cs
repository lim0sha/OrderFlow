namespace DataAccess.Models.Entities.Orders;

public enum OrderHistoryItemKind
{
    Created,
    ItemAdded,
    ItemRemoved,
    StateChanged,
}