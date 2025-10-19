using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;
using Task1.Clients.Builders;
using Task1.Clients.Interfaces;
using Task1.Models.Entities;

namespace Task1.Clients.Extensions;

public static class RefitClientExtension
{
    public static IServiceCollection IncludeRefitClient(this IServiceCollection services)
    {
        services.AddRefitClient<IRefitClient>()
            .ConfigureHttpClient(ConfigureHttpClient);

        services.AddScoped<IWebClient, RefitClient>();
        services.AddScoped<RefitClient>();
        services.AddSingleton<IQueryStringBuilder, QueryStringBuilder>();

        return services;
    }

    private static void ConfigureHttpClient(IServiceProvider provider, HttpClient httpClient)
    {
        ConnectionOptions options = provider.GetRequiredService<IOptions<ConnectionOptions>>().Value;
        httpClient.BaseAddress = new Uri($"http://{options.ConnectionHost}:{options.ConnectionPort}");
    }
}