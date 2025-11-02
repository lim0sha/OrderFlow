using DataAccess.Models.Entities.Orders;
using DataAccess.Models.Enums;
using DataAccess.Utils.Mappers.Interfaces;
using Npgsql;
using System.Data;

namespace DataAccess.Utils.Mappers;

public class OrderMapper : IOrderMapper
{
    public Order MapOrder(NpgsqlDataReader reader)
    {
        return new Order(
            reader.GetInt64("order_id"),
            reader.GetFieldValue<OrderState>("order_state"),
            reader.GetDateTime("order_created_at"),
            reader.GetString("order_created_by"));
    }
}