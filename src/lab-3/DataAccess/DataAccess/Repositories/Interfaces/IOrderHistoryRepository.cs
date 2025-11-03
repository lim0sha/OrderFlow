using DataAccess.Models.Entities.Orders;
using DataAccess.Models.Requests;

namespace DataAccess.Repositories.Interfaces;

public interface IOrderHistoryRepository
{
    Task<long> Create(OrderHistory orderHistory, CancellationToken cancellationToken);

    IAsyncEnumerable<OrderHistory> GetFiltered(
        int position,
        int volume,
        OrderHistoryRequestFiltered request,
        CancellationToken cancellationToken);
}