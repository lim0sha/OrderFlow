namespace Gateway.Models.Responses.Products.Create;

public sealed record ValidationErrorResponse(string Message) : CreateProductResponseBase;