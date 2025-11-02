using DataAccess.Models.Entities.Products;
using DataAccess.Models.Requests;

namespace DataAccess.Repositories.Interfaces;

public interface IProductRepository
{
    Task<long> Create(Product p, CancellationToken ct);

    IAsyncEnumerable<Product> GetFiltered(int token, int volume, ProductRequestFiltered request, CancellationToken ct);
}