namespace Gateway.Models.Responses.Products.Create;

public sealed record ProductNameAlreadyExistsResponse(string Message) : CreateProductResponseBase;
