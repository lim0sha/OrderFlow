using System.Text.Json.Serialization;

namespace Task1.Models.Entities;

public readonly record struct ConfigPage(
    [property: JsonPropertyName("items")] IEnumerable<Config> Configs,
    [property: JsonPropertyName("pageToken")] string? PageToken);