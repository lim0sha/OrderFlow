using DataAccess.Services.Interfaces;
using Grpc.Core;
using Presentation.Protos;
using Product = DataAccess.Models.Entities.Products.Product;

namespace Presentation.Services;

public class ProductGrpcService : ProductService.ProductServiceBase
{
    private readonly IProductService _productService;

    public ProductGrpcService(IProductService productService)
    {
        _productService = productService;
    }

    public override async Task<CreateProductResponse> CreateProduct(
        CreateProductRequest request,
        ServerCallContext context)
    {
        var product = new Product(0, request.ProductName, (decimal)request.ProductPrice);
        bool result = await _productService.Create(product, context.CancellationToken);

        return result
            ? new CreateProductResponse
            {
                Success = new CreateProductSuccess(),
            }
            : new CreateProductResponse
            {
                ValidationError = new ValidationError { Message = "Failed to create product" },
            };
    }
}