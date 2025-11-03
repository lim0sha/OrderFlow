using DataAccess.Models.Entities.Products;

namespace DataAccess.Services.Interfaces;

public interface IProductService
{
    Task<bool> Create(Product p, CancellationToken ct);
}