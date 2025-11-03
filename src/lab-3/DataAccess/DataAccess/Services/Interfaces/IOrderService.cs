using DataAccess.Models.Entities.Orders;
using DataAccess.Models.Requests;

namespace DataAccess.Services.Interfaces;

public interface IOrderService
{
    Task<bool> Create(Order o, CancellationToken ct);

    Task<bool> AddItem(OrderItem oi, CancellationToken ct);

    Task<bool> RemoveItem(long oId, long oiId, CancellationToken ct);

    Task<bool> TransferToWork(long id, CancellationToken ct);

    Task<bool> CompleteOrder(long id, CancellationToken ct);

    Task<bool> Cancel(long id, CancellationToken ct);

    IAsyncEnumerable<OrderHistory> GetHistoryByFilter(
        int position,
        int volume,
        OrderHistoryRequestFiltered request,
        CancellationToken ct);
}