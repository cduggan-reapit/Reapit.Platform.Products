using System.Text.Json.Serialization;

namespace ProductsCdk.Models;

public record ApiContext(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("logGroup")] string LogGroupName,
    [property: JsonPropertyName("dns")] string DnsNamespace,
    [property: JsonPropertyName("urlOutput")] string UrlOutputName);