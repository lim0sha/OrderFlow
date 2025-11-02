using DataAccess.Models.Entities.Orders;
using DataAccess.Utils.Mappers.Interfaces;
using Npgsql;
using System.Data;

namespace DataAccess.Utils.Mappers;

public class OrderItemMapper : IOrderItemMapper
{
    public OrderItem MapOrderItem(NpgsqlDataReader reader)
    {
        return new OrderItem(
            OrderId: reader.GetInt64("order_id"),
            ProductId: reader.GetInt64("product_id"),
            OrderItemQuantity: reader.GetInt32("order_item_quantity"),
            OrderItemDeleted: reader.GetBoolean("order_item_deleted"),
            Id: reader.GetInt64("order_item_id"));
    }
}