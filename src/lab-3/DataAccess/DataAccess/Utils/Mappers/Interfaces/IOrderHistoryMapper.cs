using DataAccess.Models.Entities.Orders;
using Npgsql;
using System.Text.Json;

namespace DataAccess.Utils.Mappers.Interfaces;

public interface IOrderHistoryMapper
{
    OrderHistory MapOrderHistory(NpgsqlDataReader reader, JsonSerializerOptions jsonOptions);
}