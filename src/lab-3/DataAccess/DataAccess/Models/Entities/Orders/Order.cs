using DataAccess.Models.Enums;

namespace DataAccess.Models.Entities.Orders;

public record struct Order(long Id, OrderState OrderState, DateTime OrderCreatedAt, string OrderCreatedBy);