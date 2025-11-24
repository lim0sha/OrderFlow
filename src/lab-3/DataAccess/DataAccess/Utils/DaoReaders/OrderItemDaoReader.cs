using DataAccess.Models.DAO;
using Npgsql;
using System.Data;

namespace DataAccess.Utils.DaoReaders;

public static class OrderItemDaoReader
{
    public static OrderItemDao Read(NpgsqlDataReader reader)
    {
        return new OrderItemDao(
            OrderItemId: reader.GetInt64("order_item_id"),
            OrderId: reader.GetInt64("order_id"),
            ProductId: reader.GetInt64("product_id"),
            OrderItemQuantity: reader.GetInt32("order_item_quantity"),
            OrderItemDeleted: reader.GetBoolean("order_item_deleted"));
    }
}