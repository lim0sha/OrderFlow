using DataAccess.Models.DAO;
using DataAccess.Models.Enums;
using Npgsql;
using System.Data;

namespace DataAccess.Utils.DaoReaders;

public static class OrderDaoReader
{
    public static OrderDao Read(NpgsqlDataReader reader)
    {
        return new OrderDao(
            OrderId: reader.GetInt64("order_id"),
            OrderState: reader.GetFieldValue<OrderState>("order_state"),
            OrderCreatedAt: reader.GetDateTime("order_created_at"),
            OrderCreatedBy: reader.GetString("order_created_by"));
    }
}