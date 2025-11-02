using Gateway.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Gateway.Extensions;

public static class GatewayExtension
{
    public static IServiceCollection AddGatewayServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GrpcConnectionOptions>(configuration.GetSection("Grpc"));

        services.AddControllers();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Gateway API", Version = "v1" });
        });

        string? orderAddress = configuration.GetValue<string>("Grpc:OrderServiceAddress");
        if (string.IsNullOrEmpty(orderAddress))
            throw new InvalidOperationException("OrderService gRPC address is not configured.");

        string? productAddress = configuration.GetValue<string>("Grpc:ProductServiceAddress");
        if (string.IsNullOrEmpty(productAddress))
            throw new InvalidOperationException("ProductService gRPC address is not configured.");

        services.AddGrpcClient<Presentation.Protos.OrderService.OrderServiceClient>(opt =>
            {
                opt.Address = new Uri(orderAddress);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            });

        services.AddGrpcClient<Presentation.Protos.ProductService.ProductServiceClient>(opt =>
            {
                opt.Address = new Uri(productAddress);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            });

        return services;
    }
}