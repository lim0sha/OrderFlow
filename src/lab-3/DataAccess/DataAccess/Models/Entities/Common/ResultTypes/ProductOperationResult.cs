using DataAccess.Models.Entities.Common.Errors;

namespace DataAccess.Models.Entities.Common.ResultTypes;

public abstract class ProductOperationResult
{
    private ProductOperationResult() { }

    public sealed class Success(long productId) : ProductOperationResult
    {
        public long ProductId { get; init; } = productId;
    }

    public sealed class ProductNameAlreadyExists(string message = "Product with this name already exists")
        : ProductOperationResult
    {
        public static ProductError ErrorType => ProductError.ProductNameAlreadyExists;

        public string ErrorMessage { get; init; } = message;
    }

    public sealed class InvalidPrice(string message = "Product price must be greater than zero")
        : ProductOperationResult
    {
        public static ProductError ErrorType => ProductError.InvalidPrice;

        public string ErrorMessage { get; init; } = message;
    }

    public sealed class ValidationError(string message)
        : ProductOperationResult
    {
        public static ProductError ErrorType => ProductError.ValidationError;

        public string ErrorMessage { get; init; } = message;
    }

    public sealed class SaveFailed(string message = "Failed to save product")
        : ProductOperationResult
    {
        public static ProductError ErrorType => ProductError.SaveFailed;

        public string ErrorMessage { get; init; } = message;
    }

    public string? GetMessage() => this switch
    {
        ProductNameAlreadyExists e => e.ErrorMessage,
        InvalidPrice e => e.ErrorMessage,
        ValidationError e => e.ErrorMessage,
        SaveFailed e => e.ErrorMessage,
        _ => null,
    };

    public ProductError? GetErrorType() => this switch
    {
        ProductNameAlreadyExists _ => ProductNameAlreadyExists.ErrorType,
        InvalidPrice _ => InvalidPrice.ErrorType,
        ValidationError _ => ValidationError.ErrorType,
        SaveFailed _ => SaveFailed.ErrorType,
        _ => null,
    };
}