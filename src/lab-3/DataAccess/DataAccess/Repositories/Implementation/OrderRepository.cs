using DataAccess.Models.Entities.Orders;
using DataAccess.Models.Requests;
using DataAccess.Repositories.Interfaces;
using DataAccess.Utils.DbUtils;
using DataAccess.Utils.Helpers.Interfaces;
using DataAccess.Utils.Mappers.Interfaces;
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

    public async Task<long> Create(Order o, CancellationToken ct)
    {
        string sql = RepositorySqlLoader.Load("Order_Create.sql");
        return await _executor.ExecuteScalarAsync<long>(
            sql,
            parameters =>
            {
                parameters.AddWithValue("order_state", o.OrderState);
                parameters.AddWithValue("order_created_at", o.OrderCreatedAt);
                parameters.AddWithValue("order_created_by", o.OrderCreatedBy);
            },
            ct);
    }

    public async Task Update(Order o, CancellationToken ct)
    {
        string sql = RepositorySqlLoader.Load("Order_Update.sql");
        await _executor.ExecuteAsync(
            sql,
            parameters =>
            {
                parameters.AddWithValue("order_id", o.Id);
                parameters.AddWithValue("order_state", o.OrderState);
                parameters.AddWithValue("order_created_at", o.OrderCreatedAt);
                parameters.AddWithValue("order_created_by", o.OrderCreatedBy);
            },
            ct);
    }

    public async Task<Order> GetById(long id, CancellationToken ct)
    {
        string sql = RepositorySqlLoader.Load("Order_GetById.sql");
        await foreach (Order order in _executor.QueryAsync(
                           sql,
                           parameters => parameters.AddWithValue("id", id),
                           reader => _orderMapper.MapOrder(reader),
                           ct))
        {
            return order;
        }

        throw new InvalidOperationException($"Order with ID {id} not found.");
    }

    public async IAsyncEnumerable<Order> GetFiltered(
        int position,
        int volume,
        OrderRequestFiltered request,
        [EnumeratorCancellation] CancellationToken ct)
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
                           ct))
        {
            yield return order;
        }
    }
}