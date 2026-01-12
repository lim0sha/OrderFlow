namespace DataAccess.Models.Entities.Orders;

public record struct OrderItem(long Id, long OrderId, long ProductId, int OrderItemQuantity, bool OrderItemDeleted);