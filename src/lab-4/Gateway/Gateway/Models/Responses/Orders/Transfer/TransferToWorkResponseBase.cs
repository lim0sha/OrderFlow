using System.Text.Json.Serialization;

namespace Gateway.Models.Responses.Orders.Transfer;

[JsonDerivedType(typeof(TransferToWorkSuccessResponse), typeDiscriminator: "success")]
[JsonDerivedType(typeof(TransferToWorkAlreadyProcessingResponse), typeDiscriminator: "alreadyProcessing")]
public abstract record TransferToWorkResponseBase;