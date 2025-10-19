using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using Task1.Clients.Interfaces;
using Task1.Models.Entities;
using Task2.Sources.Managers.Interfaces;
using Task2.Sources.Providers;
using Task2.Utils;

namespace Task2.Sources.Managers;

public class ConfigSyncManager : IConfigManager
{
    private readonly ConfigStorage _storage;
    private readonly IWebClient _client;
    private readonly UpdaterOptions _options;

    public ConfigSyncManager(ConfigStorage storage, IWebClient client, IOptions<UpdaterOptions> options)
    {
        _storage = storage;
        _client = client;
        _options = options.Value;
    }

    public async Task Update(int pageSize, string? pageToken, CancellationToken token)
    {
        await _storage.Refresh(RetrieveAsync(pageSize, pageToken, token), token);
    }

    public async Task UpdateOnInterval(int pageSize, string? pageToken, CancellationToken token)
    {
        var interval = TimeSpan.FromSeconds(_options.RefreshIntervalSeconds);
        using var loopTimer = new PeriodicTimer(interval);
        try
        {
            while (await loopTimer.WaitForNextTickAsync(token))
            {
                IAsyncEnumerable<Config> configs = _client.FetchConfigs(pageSize, pageToken, token);
                await _storage.Refresh(configs, token);
            }
        }
        catch (OperationCanceledException exception)
        {
            System.Console.WriteLine(exception.Message);
        }
    }

    public async IAsyncEnumerable<Config> RetrieveAsync(int pageSize, string? pageToken, [EnumeratorCancellation] CancellationToken token)
    {
        await foreach (Config cfg in _client.FetchConfigs(pageSize, pageToken, token))
        {
            yield return cfg;
        }
    }
}