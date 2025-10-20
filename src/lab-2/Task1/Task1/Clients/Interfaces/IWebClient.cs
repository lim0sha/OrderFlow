using Task1.Models.Entities;

namespace Task1.Clients.Interfaces;

public interface IWebClient
{
    IAsyncEnumerable<Config> FetchConfigs(int pages, string? tk, CancellationToken ct);
}