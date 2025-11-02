using DataAccess.Models.Entities.Products;
using DataAccess.Models.Requests;
using DataAccess.Repositories.Interfaces;
using DataAccess.Utils.DbUtils;
using DataAccess.Utils.Helpers.Interfaces;
using System.Data;
using System.Runtime.CompilerServices;

namespace DataAccess.Repositories.Implementation;

public class ProductRepository : IProductRepository
{
    private readonly IDbCommandExecutor _executor;

    public ProductRepository(IDbCommandExecutor executor)
    {
        _executor = executor;
    }

    public async Task<long> Create(Product p, CancellationToken ct)
    {
        string sql = RepositorySqlLoader.Load("Product_Create.sql");
        return await _executor.ExecuteScalarAsync<long>(
            sql,
            parameters =>
            {
                parameters.AddWithValue("name", p.ProductName);
                parameters.AddWithValue("price", p.ProductPrice);
            },
            ct);
    }

    public async IAsyncEnumerable<Product> GetFiltered(
        int token,
        int volume,
        ProductRequestFiltered request,
        [EnumeratorCancellation] CancellationToken ct)
    {
        string sql = RepositorySqlLoader.Load("Product_GetFiltered.sql");
        await foreach (Product product in _executor.QueryAsync(
                           sql,
                           parameters =>
                           {
                               parameters.AddWithValue("cursor", token);
                               parameters.AddWithValue("page_size", volume);
                               parameters.AddWithValue("ids", request.IdList);
                               parameters.AddWithValue("product_name", request.Title);
                               parameters.AddWithValue("min_price", request.MinimumPrice);
                               parameters.AddWithValue("max_price", request.MaximumPrice);
                           },
                           reader => new Product(
                               reader.GetInt64("product_id"),
                               reader.GetString("product_name"),
                               reader.GetDecimal("product_price")),
                           ct))
        {
            yield return product;
        }
    }
}