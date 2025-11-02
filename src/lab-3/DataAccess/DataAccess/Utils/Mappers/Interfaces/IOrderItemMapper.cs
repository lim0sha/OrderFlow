using DataAccess.Models.Entities.Orders;
using Npgsql;

namespace DataAccess.Utils.Mappers.Interfaces;

public interface IOrderItemMapper
{
    OrderItem MapOrderItem(NpgsqlDataReader reader);
}