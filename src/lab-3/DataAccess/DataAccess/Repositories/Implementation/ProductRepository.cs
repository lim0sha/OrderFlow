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

    public async Task<long> Create(Product product, CancellationToken cancellationToken)
    {
        string sql = RepositorySqlLoader.Load("Product_Create.sql");
        return await _executor.ExecuteScalarAsync<long>(
            sql,
            parameters =>
            {
                parameters.AddWithValue("name", product.ProductName);
                parameters.AddWithValue("price", product.ProductPrice);
            },
            cancellationToken);
    }

    public async IAsyncEnumerable<Product> GetFiltered(
        int token,
        int volume,
        ProductRequestFiltered request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
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
                           cancellationToken))
        {
            yield return product;
        }
    }

    public async Task<Product> GetProductByName(string name, CancellationToken cancellationToken)
    {
        string sql = RepositorySqlLoader.Load("Product_GetByName.sql");
        return await _executor.QueryFirstOrDefaultAsync(
            sql,
            parameters =>
            {
                parameters.AddWithValue("name", name);
            },
            reader => new Product(
                reader.GetInt64("product_id"),
                reader.GetString("product_name"),
                reader.GetDecimal("product_price")),
            cancellationToken);
    }
}