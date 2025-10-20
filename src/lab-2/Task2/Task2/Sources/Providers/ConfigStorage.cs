using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Task1.Models.Entities;
using Task2.Sources.Providers.Interfaces;

namespace Task2.Sources.Providers;

public class ConfigStorage : ConfigurationProvider, IConfigProvider
{
    public void Refresh(IEnumerable<Config> collection)
    {
        Dictionary<string, string> newData = new();

        foreach (Config item in collection)
        {
            newData[item.Key] = item.Value;
        }

        bool updated = false;

        var obsolete = Data.Keys.Except(newData.Keys).ToList();
        if (obsolete.Count > 0)
        {
            foreach (string oldKey in obsolete)
                Data.Remove(oldKey);
            updated = true;
        }

        foreach (KeyValuePair<string, string> pair in newData)
        {
            if (Data.TryGetValue(pair.Key, out string? current) && current == pair.Value)
                continue;

            Data[pair.Key] = pair.Value;
            updated = true;
        }

        if (updated)
            OnReload();
    }

    public IChangeToken ReloadTokenAccessor => GetReloadToken();
}