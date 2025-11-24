using DataAccess.Models.Entities.Orders;
using DataAccess.Utils.DaoReaders;
using DataAccess.Utils.Mappers.Interfaces;
using Npgsql;

namespace DataAccess.Utils.Mappers;

public class OrderItemMapper : IOrderItemMapper
{
    public OrderItem MapOrderItem(NpgsqlDataReader reader)
    {
        Models.DAO.OrderItemDao dao = OrderItemDaoReader.Read(reader);
        return new OrderItem(
            Id: dao.OrderItemId,
            OrderId: dao.OrderId,
            ProductId: dao.ProductId,
            OrderItemQuantity: dao.OrderItemQuantity,
            OrderItemDeleted: dao.OrderItemDeleted);
    }
}