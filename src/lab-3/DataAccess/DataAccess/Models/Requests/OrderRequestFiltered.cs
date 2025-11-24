using DataAccess.Models.Enums;

namespace DataAccess.Models.Requests;

public record struct OrderRequestFiltered(long[] IdList, OrderState OrderState, string CreatedBy);