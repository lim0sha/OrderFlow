using Task1.Models.Entities;

namespace Task2.Sources.Providers.Interfaces;

public interface IConfigProvider
{
    Task Refresh(IAsyncEnumerable<Config> collection, CancellationToken token);
}