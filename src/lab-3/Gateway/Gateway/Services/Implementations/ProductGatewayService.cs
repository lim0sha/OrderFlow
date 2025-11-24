using Gateway.Models.DTO.Products;
using Gateway.Models.Responses.Products.Create;
using Gateway.Services.Interfaces;
using Presentation.Protos;

namespace Gateway.Services.Implementations;

public class ProductGatewayService : IProductGatewayService
{
    private readonly ProductService.ProductServiceClient _productClient;

    public ProductGatewayService(ProductService.ProductServiceClient productClient)
    {
        _productClient = productClient;
    }

    public async Task<CreateProductResponseBase> CreateProductAsync(CreateProductRequestDto request)
    {
        var grpcRequest = new CreateProductRequest
        {
            ProductName = request.ProductName,
            ProductPrice = (double)request.ProductPrice,
        };

        CreateProductResponse grpcResponse = await _productClient.CreateProductAsync(grpcRequest);

        return grpcResponse.ResultCase switch
        {
            CreateProductResponse.ResultOneofCase.Success =>
                new CreateProductSuccessResponse(grpcResponse.Success.ProductId),

            CreateProductResponse.ResultOneofCase.ValidationError =>
                new ValidationErrorResponse(grpcResponse.ValidationError.Message),

            CreateProductResponse.ResultOneofCase.NameExists =>
                new ProductNameAlreadyExistsResponse(grpcResponse.NameExists.Message),

            CreateProductResponse.ResultOneofCase.InvalidPrice =>
                new InvalidPriceResponse(grpcResponse.InvalidPrice.Message),

            CreateProductResponse.ResultOneofCase.SaveFailed =>
                new SaveFailedResponse(grpcResponse.SaveFailed.Message),

            CreateProductResponse.ResultOneofCase.None =>
                new CreateProductNoneResponse(),

            _ => throw new InvalidOperationException("Unexpected gRPC response case"),
        };
    }
}