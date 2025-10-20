using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Task3.Models.Entities;
using Task3.Services;
using Task3.Services.Interfaces;

namespace Task3.Extensions;

public static class DrawerExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RendererBlueprint>(configuration.GetSection("Renderer"));
        services.AddHttpClient();
        services.AddSingleton<IShower, Shower>();
        services.AddHostedService<Displayer>();
        return services;
    }
}