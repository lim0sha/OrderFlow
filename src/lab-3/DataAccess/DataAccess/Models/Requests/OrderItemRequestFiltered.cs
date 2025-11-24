namespace DataAccess.Models.Requests;

public record struct OrderItemRequestFiltered(long[] OrderIds, long[] ProductIds, bool IsDeleted);