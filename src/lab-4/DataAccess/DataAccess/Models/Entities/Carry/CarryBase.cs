using System.Text.Json.Serialization;

namespace DataAccess.Models.Entities.Carry;

[JsonDerivedType(typeof(PostCarryBase), typeDiscriminator: nameof(PostCarryBase))]
[JsonDerivedType(typeof(PostItemCarryBase), typeDiscriminator: nameof(PostItemCarryBase))]
[JsonDerivedType(typeof(PutStateCarryBase), typeDiscriminator: nameof(PutStateCarryBase))]
[JsonDerivedType(typeof(DeleteItemCarryBase), typeDiscriminator: nameof(DeleteItemCarryBase))]
public abstract record CarryBase;