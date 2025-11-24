namespace DataAccess.Models.Requests;

public record struct ProductRequestFiltered(long[] IdList, decimal MinimumPrice, decimal MaximumPrice, string Title);