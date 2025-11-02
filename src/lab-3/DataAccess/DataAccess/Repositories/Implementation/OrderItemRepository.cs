using DataAccess.Models.Entities.Orders;
using DataAccess.Models.Requests;
using DataAccess.Repositories.Interfaces;
using DataAccess.Utils.DbUtils;
using DataAccess.Utils.Helpers.Interfaces;
using DataAccess.Utils.Mappers.Interfaces;
using System.Runtime.CompilerServices;

namespace DataAccess.Repositories.Implementation;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly IDbCommandExecutor _executor;
    private readonly IOrderItemMapper _orderItemMapper;

    public OrderItemRepository(IDbCommandExecutor executor, IOrderItemMapper orderItemMapper)
    {
        _executor = executor;
        _orderItemMapper = orderItemMapper;
    }

    public async Task<long> Create(OrderItem oi, CancellationToken ct)
    {
        string sql = RepositorySqlLoader.Load("OrderItem_Create.sql");
        return await _executor.ExecuteScalarAsync<long>(
            sql,
            parameters =>
            {
                parameters.AddWithValue("order_id", oi.OrderId);
                parameters.AddWithValue("product_id", oi.ProductId);
                parameters.AddWithValue("order_item_quantity", oi.OrderItemQuantity);
                parameters.AddWithValue("order_item_deleted", oi.OrderItemDeleted);
            },
            ct);
    }

    public async Task Update(OrderItem oi, CancellationToken ct)
    {
        string sql = RepositorySqlLoader.Load("OrderItem_Update.sql");
        await _executor.ExecuteAsync(
            sql,
            parameters =>
            {
                parameters.AddWithValue("order_item_id", oi.Id);
                parameters.AddWithValue("order_id", oi.OrderId);
                parameters.AddWithValue("product_id", oi.ProductId);
                parameters.AddWithValue("order_item_quantity", oi.OrderItemQuantity);
                parameters.AddWithValue("order_item_deleted", oi.OrderItemDeleted);
            },
            ct);
    }

    public async Task<OrderItem> GetById(long id, CancellationToken ct)
    {
        string sql = RepositorySqlLoader.Load("OrderItem_GetById.sql");
        await foreach (OrderItem item in _executor.QueryAsync(
                           sql,
                           parameters => parameters.AddWithValue("id", id),
                           reader => _orderItemMapper.MapOrderItem(reader),
                           ct))
        {
            return item;
        }

        throw new InvalidOperationException($"OrderItem with ID {id} not found.");
    }

    public async IAsyncEnumerable<OrderItem> GetByFilter(
        int position,
        int volume,
        OrderItemRequestFiltered request,
        [EnumeratorCancellation] CancellationToken ct)
    {
        string sql = RepositorySqlLoader.Load("OrderItem_GetByFilter.sql");
        await foreach (OrderItem item in _executor.QueryAsync(
                           sql,
                           parameters =>
                           {
                               parameters.AddWithValue("cursor", position);
                               parameters.AddWithValue("page_size", volume);
                               parameters.AddWithValue("order_ids", request.OrderIds);
                               parameters.AddWithValue("product_ids", request.ProductIds);
                               parameters.AddWithValue("order_item_deleted", request.IsDeleted);
                           },
                           reader => _orderItemMapper.MapOrderItem(reader),
                           ct))
        {
            yield return item;
        }
    }
}