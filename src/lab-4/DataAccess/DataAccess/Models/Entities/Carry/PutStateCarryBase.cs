using DataAccess.Models.Enums;

namespace DataAccess.Models.Entities.Carry;

public sealed record PutStateCarryBase(OrderState Initial, OrderState Final) : CarryBase;