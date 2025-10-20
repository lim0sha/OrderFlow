using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using Task1.Clients.Interfaces;
using Task1.Models.Entities;

namespace Task1.Clients;

public class DefaultHttpClientClient : IWebClient
{
    private readonly IHttpClientFactory _factory;
    private readonly HttpClient _httpClient;
    private readonly IQueryStringBuilder _queryBuilder;

    public DefaultHttpClientClient(
        IHttpClientFactory httpClientFactory,
        IOptions<ConnectionOptions> connectionOptions,
        IQueryStringBuilder queryBuilder)
    {
        _factory = httpClientFactory;
        _httpClient = _factory.CreateClient();
        _httpClient.BaseAddress = new Uri($"http://{connectionOptions.Value.ConnectionHost}:{connectionOptions.Value.ConnectionPort}/configurations");
        _queryBuilder = queryBuilder;
    }

    public async IAsyncEnumerable<Config> FetchConfigs(int pages, string? tk, [EnumeratorCancellation] CancellationToken ct)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { "pageVolume", pages.ToString() },
        };

        if (!string.IsNullOrEmpty(tk))
            queryParameters["tk"] = tk;

        do
        {
            Uri uri = BuildUriWithQuery(queryParameters);
            using HttpResponseMessage response = await SendHttpRequestAsync(uri, ct);
            ConfigPage page = await response.Content.ReadFromJsonAsync<ConfigPage>(ct);

            foreach (Config config in page.Configs)
                yield return config;

            tk = page.PageToken;
            if (tk != null)
                queryParameters["tk"] = tk;
        }
        while (tk != null);
    }

    private Uri BuildUriWithQuery(Dictionary<string, string> queryParameters)
    {
        string queryString = _queryBuilder.Build(queryParameters);
        return new Uri($"?{queryString}", UriKind.Relative);
    }

    private async Task<HttpResponseMessage> SendHttpRequestAsync(Uri uri, CancellationToken ct)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(uri, ct).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return response;
    }
}
