using System.Text.Json.Serialization;

namespace Gateway.Models.Responses.Orders.Complete;

[JsonDerivedType(typeof(CompleteOrderSuccessResponse), typeDiscriminator: "success")]
[JsonDerivedType(typeof(CompleteOrderAlreadyCompletedResponse), typeDiscriminator: "alreadyCompleted")]
[JsonDerivedType(typeof(CompleteOrderNotFoundResponse), typeDiscriminator: "notFound")]
public abstract record CompleteOrderResponseBase;