namespace DataAccess.Models.Entities.Products;

public record struct Product(long Id, string ProductName, decimal ProductPrice)
{
    public bool Validate()
    {
        return !string.IsNullOrWhiteSpace(ProductName) && ProductPrice > 0;
    }
}