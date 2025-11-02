namespace DataAccess.Models.Entities.Common.Errors;

public enum OrderError
{
    OrderIsNotCreated,
    OrderAlreadyProcessing,
    OrderAlreadyCompleted,
    OrderAlreadyCancelled,
    OrderCannotBeCancelled,
    OrderNotFound,
    OrderItemNotFound,
    InvalidOperation,
}