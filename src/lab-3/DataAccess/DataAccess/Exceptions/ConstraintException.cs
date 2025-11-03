namespace DataAccess.Exceptions;

public class ConstraintException : Exception
{
    public ConstraintException()
        : base("A database constraint was violated.") { }

    public ConstraintException(string message)
        : base(message) { }

    public ConstraintException(string message, Exception innerException)
        : base(message, innerException) { }
}