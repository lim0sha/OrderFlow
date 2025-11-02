using DataAccess.Models.Entities.Orders;
using DataAccess.Models.Requests;

namespace DataAccess.Repositories.Interfaces;

public interface IOrderItemRepository
{
    Task<long> Create(OrderItem oi, CancellationToken ct);

    Task<OrderItem> GetById(long id, CancellationToken ct);

    IAsyncEnumerable<OrderItem> GetByFilter(
        int position,
        int volume,
        OrderItemRequestFiltered request,
        CancellationToken ct);

    Task Update(OrderItem oi, CancellationToken ct);
}