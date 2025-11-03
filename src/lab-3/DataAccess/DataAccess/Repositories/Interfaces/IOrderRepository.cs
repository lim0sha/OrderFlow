using DataAccess.Models.Entities.Orders;
using DataAccess.Models.Requests;

namespace DataAccess.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<long> Create(Order order, CancellationToken cancellationToken);

    Task<Order> GetById(long id, CancellationToken cancellationToken);

    Task<Order> GetOrderByUser(string user, CancellationToken cancellationToken);

    Task<OrderItem> GetOrderItemByProduct(long productId, long orderId, CancellationToken cancellationToken);

    IAsyncEnumerable<Order> GetFiltered(int position, int volume, OrderRequestFiltered request, CancellationToken cancellationToken);

    Task Update(Order order, CancellationToken cancellationToken);
}