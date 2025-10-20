using Microsoft.Extensions.Configuration;
using Task2.Sources.Providers;

namespace Task2.Sources;

public class MyConfigEntitySource : IConfigurationSource
{
    private readonly ConfigStorage _storage;

    public MyConfigEntitySource(ConfigStorage storage)
    {
        _storage = storage;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return _storage;
    }
}