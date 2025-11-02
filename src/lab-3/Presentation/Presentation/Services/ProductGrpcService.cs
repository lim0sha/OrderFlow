using DataAccess.Models.Entities.Common.ResultTypes;
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
        ProductOperationResult result = await _productService.Create(product, context.CancellationToken);

        return result switch
        {
            ProductOperationResult.Success s => new CreateProductResponse
            {
                Success = new CreateProductSuccess { ProductId = s.ProductId },
            },
            ProductOperationResult.ValidationError e => new CreateProductResponse
            {
                ValidationError = new ValidationError { Message = e.ErrorMessage },
            },
            ProductOperationResult.ProductNameAlreadyExists e => new CreateProductResponse
            {
                NameExists = new ProductNameAlreadyExists { Message = e.ErrorMessage },
            },
            ProductOperationResult.InvalidPrice e => new CreateProductResponse
            {
                InvalidPrice = new InvalidPrice { Message = e.ErrorMessage },
            },
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown error")),
        };
    }
}