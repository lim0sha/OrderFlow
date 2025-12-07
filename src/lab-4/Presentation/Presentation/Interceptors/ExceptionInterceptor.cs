using DataAccess.Exceptions;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Presentation.Interceptors;

public class ExceptionInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (RpcException)
        {
            throw;
        }
        catch (ConstraintException ex)
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, ex.Message),
                $"Constraint violation: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[gRPC Internal Error:] {ex.GetType().Name}: {ex.Message}");
            Console.WriteLine(ex.StackTrace);

            throw new RpcException(
                new Status(StatusCode.Internal, "Internal server error"),
                $"Unexpected error: {ex.Message}");
        }
    }
}