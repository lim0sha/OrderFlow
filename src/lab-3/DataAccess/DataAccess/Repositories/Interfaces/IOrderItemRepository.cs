using DataAccess.Models.Entities.Orders;
using DataAccess.Models.Requests;

namespace DataAccess.Repositories.Interfaces;

public interface IOrderItemRepository
{
    Task<long> Create(OrderItem orderItem, CancellationToken cancellationToken);

    Task<OrderItem> GetById(long id, CancellationToken cancellationToken);

    IAsyncEnumerable<OrderItem> GetByFilter(
        int position,
        int volume,
        OrderItemRequestFiltered request,
        CancellationToken cancellationToken);

    Task Update(OrderItem orderItem, CancellationToken cancellationToken);
}