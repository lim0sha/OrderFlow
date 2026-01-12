using Gateway.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Presentation.Protos;

namespace Gateway.Extensions;

public static class GatewayExtension
{
    public static IServiceCollection AddGatewayServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GrpcConnectionOptions>(configuration.GetSection("Grpc"));

        GrpcConnectionOptions grpcOptions = configuration.GetSection("Grpc").Get<GrpcConnectionOptions>() ?? throw new InvalidOperationException();
        if (string.IsNullOrEmpty(grpcOptions.OrderServiceAddress))
            throw new InvalidOperationException("OrderService gRPC address is not configured.");

        if (string.IsNullOrEmpty(grpcOptions.ProductServiceAddress))
            throw new InvalidOperationException("ProductService gRPC address is not configured.");

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Gateway API",
                Version = "v1",
            });
        });

        services.AddGrpcClient<OrderService.OrderServiceClient>((sp, options) =>
            {
                GrpcConnectionOptions grpcConfig = sp.GetRequiredService<IOptions<GrpcConnectionOptions>>().Value;
                options.Address = new Uri(grpcConfig.OrderServiceAddress ?? throw new InvalidOperationException());
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            });

        services.AddGrpcClient<ProductService.ProductServiceClient>((sp, options) =>
            {
                GrpcConnectionOptions grpcConfig = sp.GetRequiredService<IOptions<GrpcConnectionOptions>>().Value;
                options.Address = new Uri(grpcConfig.ProductServiceAddress ?? throw new InvalidOperationException());
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            });

        return services;
    }
}