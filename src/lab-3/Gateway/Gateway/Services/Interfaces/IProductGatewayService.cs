using Gateway.Models.DTO.Products;
using Gateway.Models.Responses.Products.Create;

namespace Gateway.Services.Interfaces;

public interface IProductGatewayService
{
    Task<CreateProductResponseBase> CreateProductAsync(CreateProductRequestDto request);
}