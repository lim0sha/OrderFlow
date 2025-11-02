using DataAccess.Models.Entities.Common.Errors;

namespace DataAccess.Models.Entities.Common.ResultTypes;

public abstract class OrderOperationResult
{
    private OrderOperationResult() { }

    public sealed class Success(long? orderItemId = null) : OrderOperationResult
    {
        public long? OrderItemId { get; init; } = orderItemId;
    }

    public sealed class OrderIsNotCreated(string message = "Order is not in 'created' state")
        : OrderOperationResult
    {
        public static OrderError ErrorType => OrderError.OrderIsNotCreated;

        public string ErrorMessage { get; init; } = message;
    }

    public sealed class OrderAlreadyProcessing(string message = "Order is already being processed")
        : OrderOperationResult
    {
        public static OrderError ErrorType => OrderError.OrderAlreadyProcessing;

        public string ErrorMessage { get; init; } = message;
    }

    public sealed class OrderAlreadyCompleted(string message = "Order is already completed")
        : OrderOperationResult
    {
        public static OrderError ErrorType => OrderError.OrderAlreadyCompleted;

        public string ErrorMessage { get; init; } = message;
    }

    public sealed class OrderAlreadyCancelled(string message = "Order is already cancelled")
        : OrderOperationResult
    {
        public static OrderError ErrorType => OrderError.OrderAlreadyCancelled;

        public string ErrorMessage { get; init; } = message;
    }

    public sealed class OrderCannotBeCancelled(string message = "Order cannot be cancelled at this stage")
        : OrderOperationResult
    {
        public static OrderError ErrorType => OrderError.OrderCannotBeCancelled;

        public string ErrorMessage { get; init; } = message;
    }

    public sealed class OrderNotFound(string message = "Order not found")
        : OrderOperationResult
    {
        public static OrderError ErrorType => OrderError.OrderNotFound;

        public string ErrorMessage { get; init; } = message;
    }

    public sealed class OrderItemNotFound(string message = "Order item not found")
        : OrderOperationResult
    {
        public static OrderError ErrorType => OrderError.OrderItemNotFound;

        public string ErrorMessage { get; init; } = message;
    }

    public sealed class InvalidOperation(string message = "Invalid operation requested")
        : OrderOperationResult
    {
        public static OrderError ErrorType => OrderError.InvalidOperation;

        public string ErrorMessage { get; init; } = message;
    }

    public string? GetMessage() => this switch
    {
        OrderIsNotCreated e => e.ErrorMessage,
        OrderAlreadyProcessing e => e.ErrorMessage,
        OrderAlreadyCompleted e => e.ErrorMessage,
        OrderAlreadyCancelled e => e.ErrorMessage,
        OrderCannotBeCancelled e => e.ErrorMessage,
        OrderNotFound e => e.ErrorMessage,
        OrderItemNotFound e => e.ErrorMessage,
        InvalidOperation e => e.ErrorMessage,
        _ => null,
    };

    public OrderError? GetErrorType() => this switch
    {
        OrderIsNotCreated _ => OrderIsNotCreated.ErrorType,
        OrderAlreadyProcessing _ => OrderAlreadyProcessing.ErrorType,
        OrderAlreadyCompleted _ => OrderAlreadyCompleted.ErrorType,
        OrderAlreadyCancelled _ => OrderAlreadyCancelled.ErrorType,
        OrderCannotBeCancelled _ => OrderCannotBeCancelled.ErrorType,
        OrderNotFound _ => OrderNotFound.ErrorType,
        OrderItemNotFound _ => OrderItemNotFound.ErrorType,
        InvalidOperation _ => InvalidOperation.ErrorType,
        _ => null,
    };
}