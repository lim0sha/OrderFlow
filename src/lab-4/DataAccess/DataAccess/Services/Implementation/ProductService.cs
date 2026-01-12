using DataAccess.Exceptions;
using DataAccess.Models.Entities.Products;
using DataAccess.Repositories.Interfaces;
using DataAccess.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace DataAccess.Services.Implementation;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductRepository productRepository,
        ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<bool> Create(Product p, CancellationToken ct)
    {
        if (!p.Validate())
        {
            return false;
        }

        try
        {
            await _productRepository.Create(p, ct);
            return true;
        }
        catch (NpgsqlException ex) when (ex.SqlState == "23505")
        {
            _logger.LogWarning(ex, "Product creation failed because of duplicate name: {ProductName}", p.ProductName);
            throw new ConstraintException("ex.SqlState == \"23505\" error");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during product creation");
            return false;
        }
    }
}