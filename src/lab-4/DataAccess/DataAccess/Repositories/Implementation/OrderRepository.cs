using DataAccess.Models.Entities.Orders;
using DataAccess.Models.Requests;
using DataAccess.Repositories.Interfaces;
using DataAccess.Utils.DbUtils;
using DataAccess.Utils.Helpers.Interfaces;
using DataAccess.Utils.Mappers.Interfaces;
using System.Data;
using System.Runtime.CompilerServices;

namespace DataAccess.Repositories.Implementation;

public class OrderRepository : IOrderRepository
{
    private readonly IDbCommandExecutor _executor;
    private readonly IOrderMapper _orderMapper;

    public OrderRepository(IDbCommandExecutor executor, IOrderMapper orderMapper)
    {
        _executor = executor;
        _orderMapper = orderMapper;
    }

    public async Task<long> Create(Order order, CancellationToken cancellationToken)
    {
        string sql = RepositorySqlLoader.Load("Order_Create.sql");
        return await _executor.ExecuteScalarAsync<long>(
            sql,
            parameters =>
            {
                parameters.AddWithValue("order_state", order.OrderState);
                parameters.AddWithValue("order_created_at", order.OrderCreatedAt);
                parameters.AddWithValue("order_created_by", order.OrderCreatedBy);
            },
            cancellationToken);
    }

    public async Task Update(Order order, CancellationToken cancellationToken)
    {
        string sql = RepositorySqlLoader.Load("Order_Update.sql");
        await _executor.ExecuteAsync(
            sql,
            parameters =>
            {
                parameters.AddWithValue("order_id", order.Id);
                parameters.AddWithValue("order_state", order.OrderState);
                parameters.AddWithValue("order_created_at", order.OrderCreatedAt);
                parameters.AddWithValue("order_created_by", order.OrderCreatedBy);
            },
            cancellationToken);
    }

    public async Task<Order> GetById(long id, CancellationToken cancellationToken)
    {
        string sql = RepositorySqlLoader.Load("Order_GetById.sql");
        await foreach (Order order in _executor.QueryAsync(
                           sql,
                           parameters => parameters.AddWithValue("id", id),
                           reader => _orderMapper.MapOrder(reader),
                           cancellationToken))
        {
            return order;
        }

        throw new InvalidOperationException($"Order with ID {id} not found.");
    }

    public async IAsyncEnumerable<Order> GetFiltered(
        int position,
        int volume,
        OrderRequestFiltered request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        string sql = RepositorySqlLoader.Load("Order_GetFiltered.sql");
        await foreach (Order order in _executor.QueryAsync(
                           sql,
                           parameters =>
                           {
                               parameters.AddWithValue("cursor", position);
                               parameters.AddWithValue("page_size", volume);
                               parameters.AddWithValue("ids", request.IdList);
                               parameters.AddWithValue("order_state", request.OrderState);
                               parameters.AddWithValue("order_created_by", request.CreatedBy);
                           },
                           reader => _orderMapper.MapOrder(reader),
                           cancellationToken))
        {
            yield return order;
        }
    }

    public async Task<Order> GetOrderByUser(string user, CancellationToken cancellationToken)
    {
        string sql = RepositorySqlLoader.Load("Order_GetByUser.sql");
        return await _executor.QueryFirstOrDefaultAsync(
            sql,
            parameters => parameters.AddWithValue("user", user),
            reader => _orderMapper.MapOrder(reader),
            cancellationToken);
    }

    public async Task<OrderItem> GetOrderItemByProduct(long productId, long orderId, CancellationToken cancellationToken)
    {
        string sql = RepositorySqlLoader.Load("OrderItem_GetByProduct.sql");
        return await _executor.QueryFirstOrDefaultAsync(
            sql,
            parameters =>
            {
                parameters.AddWithValue("product_id", productId);
                parameters.AddWithValue("order_id", orderId);
            },
            reader => new OrderItem(
                reader.GetInt64("order_item_id"),
                reader.GetInt64("order_id"),
                reader.GetInt64("product_id"),
                reader.GetInt32("order_item_quantity"),
                reader.GetBoolean("order_item_deleted")),
            cancellationToken);
    }
}