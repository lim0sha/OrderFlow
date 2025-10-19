using Task1.Models.Entities;

namespace Task2.Sources.Managers.Interfaces;

public interface IConfigManager
{
    Task Update(int pageSize, string? pageToken, CancellationToken token);

    Task UpdateOnInterval(int pageSize, string? pageToken, CancellationToken token);

    IAsyncEnumerable<Config> RetrieveAsync(
        int pageSize,
        string? pageToken,
        CancellationToken token);
}