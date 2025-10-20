using Task1.Models.Entities;

namespace Task2.Sources.Providers.Interfaces;

public interface IConfigProvider
{
    void Refresh(IEnumerable<Config> collection);
}