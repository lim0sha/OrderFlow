using DataAccess.Models.Entities.Products;
using DataAccess.Models.Requests;

namespace DataAccess.Repositories.Interfaces;

public interface IProductRepository
{
    Task<long> Create(Product product, CancellationToken cancellationToken);

    Task<Product> GetProductByName(string name, CancellationToken cancellationToken);

    IAsyncEnumerable<Product> GetFiltered(int token, int volume, ProductRequestFiltered request, CancellationToken cancellationToken);
}