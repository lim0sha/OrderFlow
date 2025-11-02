using Microsoft.Extensions.DependencyInjection;
using Presentation.Interceptors;
using Presentation.Services;

namespace Presentation.Extensions;

public static class GrpcServiceExtensions
{
    public static IServiceCollection AddGrpcServices(this IServiceCollection services)
    {
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<ExceptionInterceptor>();
        });

        services.AddScoped<ProductGrpcService>();
        services.AddScoped<OrderGrpcService>();

        return services;
    }
}