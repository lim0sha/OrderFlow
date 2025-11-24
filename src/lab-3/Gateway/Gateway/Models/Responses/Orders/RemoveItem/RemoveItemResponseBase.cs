using System.Text.Json.Serialization;

namespace Gateway.Models.Responses.Orders.RemoveItem;

[JsonDerivedType(typeof(RemoveItemSuccessResponse), typeDiscriminator: nameof(RemoveItemSuccessResponse))]
[JsonDerivedType(typeof(RemoveItemOrderNotFoundResponse), typeDiscriminator: nameof(RemoveItemOrderNotFoundResponse))]
[JsonDerivedType(typeof(RemoveItemOrderItemNotFoundResponse), typeDiscriminator: nameof(RemoveItemOrderItemNotFoundResponse))]
public abstract record RemoveItemResponseBase;