using DataAccess.Models.Entities.Orders;
using DataAccess.Models.Requests;

namespace DataAccess.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<long> Create(Order o, CancellationToken ct);

    Task<Order> GetById(long id, CancellationToken ct);

    IAsyncEnumerable<Order> GetFiltered(int position, int volume, OrderRequestFiltered request, CancellationToken ct);

    Task Update(Order o, CancellationToken ct);
}