using DataAccess.Models.Entities.Orders;
using DataAccess.Utils.DaoReaders;
using DataAccess.Utils.Mappers.Interfaces;
using Npgsql;

namespace DataAccess.Utils.Mappers;

public class OrderMapper : IOrderMapper
{
    public Order MapOrder(NpgsqlDataReader reader)
    {
        Models.DAO.OrderDao dao = OrderDaoReader.Read(reader);
        return new Order(
            Id: dao.OrderId,
            OrderState: dao.OrderState,
            OrderCreatedAt: dao.OrderCreatedAt,
            OrderCreatedBy: dao.OrderCreatedBy);
    }
}