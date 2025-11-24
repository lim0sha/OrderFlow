using DataAccess.Models.Entities.Orders;
using DataAccess.Models.Requests;
using DataAccess.Repositories.Interfaces;
using DataAccess.Utils.DbUtils;
using DataAccess.Utils.Helpers.Interfaces;
using DataAccess.Utils.Mappers;
using DataAccess.Utils.Mappers.Interfaces;
using Npgsql;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace DataAccess.Repositories.Implementation;

public class OrderHistoryRepository : IOrderHistoryRepository
{
    private readonly IDbCommandExecutor _executor;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly IOrderHistoryMapper _orderHistoryMapper;

    public OrderHistoryRepository(
        IDbCommandExecutor executor,
        JsonSerializerOptions? jsonOptions = null,
        IOrderHistoryMapper? orderHistoryMapper = null)
    {
        _executor = executor;
        _jsonOptions = jsonOptions ?? new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
        };
        _orderHistoryMapper = orderHistoryMapper ?? new OrderHistoryMapper();
    }

    public async Task<long> Create(OrderHistory orderHistory, CancellationToken cancellationToken)
    {
        string sql = RepositorySqlLoader.Load("OrderHistory_Create.sql");
        return await _executor.ExecuteScalarAsync<long>(
            sql,
            parameters =>
            {
                parameters.AddWithValue("order_id", orderHistory.OrderId);
                parameters.AddWithValue("order_history_item_created_at", orderHistory.OrderHistoryItemCreatedAt);
                parameters.AddWithValue("order_history_item_kind", orderHistory.OrderHistoryItemKind);
                parameters.AddWithValue("order_history_item_payload", JsonSerializer.Serialize(orderHistory.OrderHistoryItemPayload, _jsonOptions));
            },
            cancellationToken);
    }

    public async IAsyncEnumerable<OrderHistory> GetFiltered(
        int position,
        int volume,
        OrderHistoryRequestFiltered request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        string sql = RepositorySqlLoader.Load("OrderHistory_GetFiltered.sql");
        await foreach (OrderHistory history in _executor.QueryAsync(
                           sql,
                           parameters =>
                           {
                               parameters.AddWithValue("cursor", position);
                               parameters.AddWithValue("page_size", volume);
                               parameters.Add(new NpgsqlParameter<long?>("order_id", request.Id));
                               parameters.Add(new NpgsqlParameter<OrderHistoryItemKind?>("order_history_item_kind", request.OrderHistoryItemKind));
                           },
                           reader => _orderHistoryMapper.MapOrderHistory(reader, _jsonOptions),
                           cancellationToken))
        {
            yield return history;
        }
    }
}