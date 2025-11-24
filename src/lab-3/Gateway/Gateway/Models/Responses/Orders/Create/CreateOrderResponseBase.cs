using System.Text.Json.Serialization;

namespace Gateway.Models.Responses.Orders.Create;

[JsonDerivedType(typeof(CreateOrderSuccessResponse), typeDiscriminator: nameof(CreateOrderSuccessResponse))]
[JsonDerivedType(typeof(OrderIsNotCreatedResponse), typeDiscriminator: nameof(OrderIsNotCreatedResponse))]
[JsonDerivedType(typeof(OrderNotFoundResponse), typeDiscriminator: nameof(OrderNotFoundResponse))]
public abstract record CreateOrderResponseBase;