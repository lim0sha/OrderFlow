using DataAccess.Models.Entities.Common.ResultTypes;
using DataAccess.Models.Entities.Orders;
using DataAccess.Models.Requests;

namespace DataAccess.Services.Interfaces;

public interface IOrderService
{
    Task<OrderOperationResult> Create(Order o, CancellationToken ct);

    Task<OrderOperationResult> AddItem(OrderItem oi, CancellationToken ct);

    Task<OrderOperationResult> RemoveItem(long oId, long oiId, CancellationToken ct);

    Task<OrderOperationResult> TransferToWork(long id, CancellationToken ct);

    Task<OrderOperationResult> CompleteOrder(long id, CancellationToken ct);

    Task<OrderOperationResult> Cancel(long id, CancellationToken ct);

    IAsyncEnumerable<OrderHistory> GetHistoryByFilter(
        int position,
        int volume,
        OrderHistoryRequestFiltered request,
        CancellationToken ct);
}