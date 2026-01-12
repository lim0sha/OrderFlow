using DataAccess.Models.Entities.Orders;
using Npgsql;

namespace DataAccess.Utils.Mappers.Interfaces;

public interface IOrderMapper
{
    Order MapOrder(NpgsqlDataReader reader);
}