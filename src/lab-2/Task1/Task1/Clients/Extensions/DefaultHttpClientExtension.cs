using Microsoft.Extensions.DependencyInjection;
using Task1.Clients.Builders;
using Task1.Clients.Interfaces;

namespace Task1.Clients.Extensions;

public static class DefaultHttpClientExtension
{
    public static IServiceCollection IncludeDefaultHttpClient(this IServiceCollection services)
    {
        services.AddScoped<IWebClient, DefaultHttpClientClient>();
        services.AddSingleton<IQueryStringBuilder, QueryStringBuilder>();
        return services;
    }
}