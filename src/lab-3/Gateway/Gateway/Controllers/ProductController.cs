using Gateway.Models.DTO.Products;
using Gateway.Models.Responses.Products.Create;
using Gateway.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductGatewayService _productService;

    public ProductController(IProductGatewayService productService)
    {
        _productService = productService;
    }

    [HttpPost("create")]
    [SwaggerResponse(StatusCodes.Status200OK, "Product created successfully", typeof(CreateProductSuccessResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation error", typeof(ValidationErrorResponse))]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Product name already exists", typeof(ProductNameAlreadyExistsResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid price", typeof(InvalidPriceResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Save failed", typeof(SaveFailedResponse))]
    public async Task<ActionResult<CreateProductResponseBase>> CreateProduct([FromBody] CreateProductRequestDto request)
    {
        CreateProductResponseBase response = await _productService.CreateProductAsync(request);

        return response switch
        {
            CreateProductSuccessResponse => Ok(response),
            ValidationErrorResponse => BadRequest(response),
            ProductNameAlreadyExistsResponse => Conflict(response),
            InvalidPriceResponse => BadRequest(response),
            SaveFailedResponse => StatusCode(StatusCodes.Status500InternalServerError, response),
            CreateProductNoneResponse => NotFound(response),
            _ => StatusCode(StatusCodes.Status500InternalServerError),
        };
    }
}