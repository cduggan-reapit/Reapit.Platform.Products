using System.Text.Json.Serialization;

namespace ProductsCdk.Models;

public record DatabaseContext(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("secret")] string Secret)
{
    public string InstanceName => $"{Name}-db";
};