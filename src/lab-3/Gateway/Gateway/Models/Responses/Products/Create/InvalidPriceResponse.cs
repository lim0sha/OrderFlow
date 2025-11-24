namespace Gateway.Models.Responses.Products.Create;

public sealed record InvalidPriceResponse(string Message) : CreateProductResponseBase;