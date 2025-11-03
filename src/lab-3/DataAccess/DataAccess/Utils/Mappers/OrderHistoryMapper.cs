using DataAccess.Models.Entities.Carry;
using DataAccess.Models.Entities.Orders;
using DataAccess.Utils.DaoReaders;
using DataAccess.Utils.Mappers.Interfaces;
using Npgsql;
using System.Text.Json;

namespace DataAccess.Utils.Mappers;

public class OrderHistoryMapper : IOrderHistoryMapper
{
    public OrderHistory MapOrderHistory(NpgsqlDataReader reader, JsonSerializerOptions jsonOptions)
    {
        Models.DAO.OrderHistoryDao dao = OrderHistoryDaoReader.Read(reader);

        CarryBase? payload = null;
        if (dao.OrderHistoryItemPayload is not null)
        {
            payload = JsonSerializer.Deserialize<CarryBase>(dao.OrderHistoryItemPayload, jsonOptions);
        }

        return new OrderHistory(
            Id: dao.OrderHistoryItemId,
            OrderId: dao.OrderId,
            OrderHistoryItemCreatedAt: dao.OrderHistoryItemCreatedAt,
            OrderHistoryItemKind: dao.OrderHistoryItemKind,
            OrderHistoryItemPayload: payload);
    }
}