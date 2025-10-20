using Microsoft.Extensions.Options;
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
        List<Config> configs = await RetrieveAllAsync(pageSize, pageToken, token);
        _storage.Refresh(configs);
    }

    public async Task UpdateOnInterval(int pageSize, string? pageToken, CancellationToken token)
    {
        var interval = TimeSpan.FromSeconds(_options.RefreshIntervalSeconds);
        using var loopTimer = new PeriodicTimer(interval);
        try
        {
            while (await loopTimer.WaitForNextTickAsync(token))
            {
                List<Config> configs = await RetrieveAllAsync(pageSize, pageToken, token);
                _storage.Refresh(configs);
            }
        }
        catch (OperationCanceledException exception)
        {
            System.Console.WriteLine(exception.Message);
        }
    }

    private async Task<List<Config>> RetrieveAllAsync(int pageSize, string? pageToken, CancellationToken token)
    {
        List<Config> result = [];
        await foreach (Config cfg in _client.FetchConfigs(pageSize, pageToken, token))
        {
            result.Add(cfg);
        }

        return result;
    }
}