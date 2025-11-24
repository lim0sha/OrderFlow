using System.Text.Json.Serialization;

namespace Gateway.Models.Responses.Orders.AddItem;

[JsonDerivedType(typeof(AddItemSuccessResponse), typeDiscriminator: nameof(AddItemSuccessResponse))]
[JsonDerivedType(typeof(AddItemOrderNotFoundResponse), typeDiscriminator: nameof(AddItemOrderNotFoundResponse))]
[JsonDerivedType(typeof(AddItemOrderIsNotCreatedResponse), typeDiscriminator: nameof(AddItemOrderIsNotCreatedResponse))]
public abstract record AddItemResponseBase;