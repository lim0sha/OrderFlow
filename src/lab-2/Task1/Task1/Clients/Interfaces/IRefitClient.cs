using Refit;
using Task1.Models.Entities;

namespace Task1.Clients.Interfaces;

public interface IRefitClient
{
    [Get("/configurations")]
    Task<ConfigPage> GetConfigPages(int pageSize, string? pageToken);
}