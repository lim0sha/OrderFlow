namespace DataAccess.Models.DAO;

public record OrderItemDao(
    long OrderItemId,
    long OrderId,
    long ProductId,
    int OrderItemQuantity,
    bool OrderItemDeleted);