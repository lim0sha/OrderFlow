using DataAccess.Models.Enums;

namespace DataAccess.Models.DAO;

public record OrderDao(
    long OrderId,
    OrderState OrderState,
    DateTime OrderCreatedAt,
    string OrderCreatedBy);