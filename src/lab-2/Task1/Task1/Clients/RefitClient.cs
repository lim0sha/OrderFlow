using System.Runtime.CompilerServices;
using Task1.Clients.Interfaces;
using Task1.Models.Entities;

namespace Task1.Clients;

public class RefitClient : IWebClient
{
    private readonly IRefitClient _refitClient;

    public RefitClient(IRefitClient refitClient)
    {
        _refitClient = refitClient;
    }

    public async IAsyncEnumerable<Config> FetchConfigs(
        int pages,
        string? tk,
        [EnumeratorCancellation] CancellationToken ct)
    {
        await foreach (Config config in FetchAllPagesAsync(pages, tk, ct))
        {
            yield return config;
        }
    }

    private async IAsyncEnumerable<Config> FetchAllPagesAsync(
        int pages,
        string? tk,
        [EnumeratorCancellation] CancellationToken ct)
    {
        while (true)
        {
            ConfigPage page = await GetPageAsync(pages, tk, ct);
            foreach (Config config in page.Configs)
            {
                yield return config;
            }

            tk = page.PageToken;
            if (tk == null)
                yield break;
        }
    }

    private async Task<ConfigPage> GetPageAsync(int pages, string? pageToken, CancellationToken ct)
    {
        return await _refitClient.GetConfigPages(pages, pageToken);
    }
}