using System.Text.Json.Serialization;

namespace Gateway.Models.Responses.Orders.Cancel;

[JsonDerivedType(typeof(CancelOrderSuccessResponse), typeDiscriminator: "success")]
[JsonDerivedType(typeof(CancelOrderAlreadyCancelledResponse), typeDiscriminator: "alreadyCancelled")]
public abstract record CancelOrderResponseBase;