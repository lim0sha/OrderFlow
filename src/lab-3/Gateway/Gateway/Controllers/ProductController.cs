using Gateway.Models.DTO.Products;
using Gateway.Models.Responses.Products.Create;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Protos;
using Swashbuckle.AspNetCore.Annotations;

namespace Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ProductService.ProductServiceClient _productClient;

    public ProductController(ProductService.ProductServiceClient productClient)
    {
        _productClient = productClient;
    }

    [HttpPost("create")]
    [SwaggerResponse(StatusCodes.Status200OK, "Product created successfully", typeof(CreateProductSuccessResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(ValidationErrorResponse))]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Product name already exists", typeof(ProductNameAlreadyExistsResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid price", typeof(InvalidPriceResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Save failed", typeof(SaveFailedResponse))]
    public async Task<ActionResult<CreateProductResponseBase>> CreateProduct([FromBody] CreateProductRequestDto request)
    {
        CreateProductResponse response = await _productClient.CreateProductAsync(new CreateProductRequest
        {
            ProductName = request.ProductName,
            ProductPrice = (double)request.ProductPrice,
        });

        return response.ResultCase switch
        {
            CreateProductResponse.ResultOneofCase.Success =>
                Ok(new CreateProductSuccessResponse(response.Success.ProductId)),

            CreateProductResponse.ResultOneofCase.ValidationError =>
                BadRequest(new ValidationErrorResponse(response.ValidationError.Message)),

            CreateProductResponse.ResultOneofCase.NameExists =>
                Conflict(new ProductNameAlreadyExistsResponse(response.NameExists.Message)),

            CreateProductResponse.ResultOneofCase.InvalidPrice =>
                BadRequest(new InvalidPriceResponse(response.InvalidPrice.Message)),

            CreateProductResponse.ResultOneofCase.SaveFailed =>
                StatusCode(StatusCodes.Status500InternalServerError, new SaveFailedResponse(response.SaveFailed.Message)),

            CreateProductResponse.ResultOneofCase.None =>
                NotFound(new CreateProductNoneResponse()),

            _ => StatusCode(StatusCodes.Status500InternalServerError),
        };
    }
}
