using DataAccess.Models.Entities.Carry;
using DataAccess.Models.Entities.Orders;
using DataAccess.Utils.Mappers.Interfaces;
using Npgsql;
using System.Data;
using System.Text.Json;

namespace DataAccess.Utils.Mappers;

public class OrderHistoryMapper : IOrderHistoryMapper
{
    public OrderHistory MapOrderHistory(NpgsqlDataReader reader, JsonSerializerOptions jsonOptions)
    {
        string payloadJson = reader.GetString("order_history_item_payload");
        CarryBase? payload = JsonSerializer.Deserialize<CarryBase?>(payloadJson, jsonOptions);

        return new OrderHistory(
            Id: reader.GetInt64("order_history_item_id"),
            OrderId: reader.GetInt64("order_id"),
            OrderHistoryItemCreatedAt: reader.GetDateTime("order_history_item_created_at"),
            OrderHistoryItemKind: reader.GetFieldValue<OrderHistoryItemKind>("order_history_item_kind"),
            OrderHistoryItemPayload: payload);
    }
}