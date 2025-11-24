using DataAccess.Models.DAO;
using DataAccess.Models.Entities.Orders;
using Npgsql;
using System.Data;

namespace DataAccess.Utils.DaoReaders;

public static class OrderHistoryDaoReader
{
    public static OrderHistoryDao Read(NpgsqlDataReader reader)
    {
        return new OrderHistoryDao(
            OrderHistoryItemId: reader.GetInt64("order_history_item_id"),
            OrderId: reader.GetInt64("order_id"),
            OrderHistoryItemCreatedAt: reader.GetDateTime("order_history_item_created_at"),
            OrderHistoryItemKind: reader.GetFieldValue<OrderHistoryItemKind>("order_history_item_kind"),
            OrderHistoryItemPayload: reader.IsDBNull("order_history_item_payload") ? null : reader.GetString("order_history_item_payload"));
    }
}