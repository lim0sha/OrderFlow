using DataAccess.Models.Entities.Common.ResultTypes;
using DataAccess.Models.Entities.Products;

namespace DataAccess.Services.Interfaces;

public interface IProductService
{
    Task<ProductOperationResult> Create(Product p, CancellationToken ct);
}