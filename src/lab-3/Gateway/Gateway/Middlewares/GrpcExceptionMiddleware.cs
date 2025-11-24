using Grpc.Core;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace Gateway.Middlewares;

public class GrpcExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GrpcExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (RpcException rpcEx)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = rpcEx.StatusCode switch
            {
                StatusCode.OK => (int)HttpStatusCode.OK,
                StatusCode.Cancelled => (int)HttpStatusCode.RequestTimeout,
                StatusCode.Unknown => (int)HttpStatusCode.InternalServerError,
                StatusCode.InvalidArgument => (int)HttpStatusCode.BadRequest,
                StatusCode.DeadlineExceeded => (int)HttpStatusCode.RequestTimeout,
                StatusCode.AlreadyExists => (int)HttpStatusCode.Conflict,
                StatusCode.PermissionDenied => (int)HttpStatusCode.Forbidden,
                StatusCode.Unauthenticated => (int)HttpStatusCode.Unauthorized,
                StatusCode.ResourceExhausted => (int)HttpStatusCode.TooManyRequests,
                StatusCode.FailedPrecondition => (int)HttpStatusCode.PreconditionFailed,
                StatusCode.Aborted => (int)HttpStatusCode.Conflict,
                StatusCode.OutOfRange => (int)HttpStatusCode.BadRequest,
                StatusCode.Unimplemented => (int)HttpStatusCode.NotImplemented,
                StatusCode.Internal => (int)HttpStatusCode.InternalServerError,
                StatusCode.Unavailable => (int)HttpStatusCode.ServiceUnavailable,
                StatusCode.DataLoss => (int)HttpStatusCode.InternalServerError,
                StatusCode.NotFound => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.InternalServerError,
            };

            var payload = new { error = rpcEx.Status.Detail };
            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}
