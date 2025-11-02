using DataAccess.Models.Entities.Common.ResultTypes;
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

    public async Task<ProductOperationResult> Create(Product p, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(p.ProductName))
        {
            return new ProductOperationResult.ValidationError("Product name is required.");
        }

        if (p.ProductPrice <= 0)
        {
            return new ProductOperationResult.InvalidPrice();
        }

        try
        {
            long productId = await _productRepository.Create(p, ct);
            return new ProductOperationResult.Success(productId);
        }
        catch (NpgsqlException ex) when (ex.SqlState == "23505")
        {
            _logger.LogWarning(ex, "Product creation failed because of duplicate name: {ProductName}", p.ProductName);
            return new ProductOperationResult.ProductNameAlreadyExists();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during product creation");
            return new ProductOperationResult.SaveFailed("An unexpected error occurred while saving the product.");
        }
    }
}