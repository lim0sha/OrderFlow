using DataAccess.Services.Implementation;
using DataAccess.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Extensions;

public static class ServicesExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}