using System.Text.Json.Serialization;

namespace Task1.Models.Entities;

public readonly record struct Config(
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("value")] string Value);