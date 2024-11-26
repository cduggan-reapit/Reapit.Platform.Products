using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Infrastructure.Logging;

/// <summary>Model representing the logger message structure for received requests.</summary>
/// <param name="Method">The request method (e.g. GET, PUT, POST).</param>
/// <param name="Url">The request URL.</param>
/// <param name="Headers">The request headers.</param>
/// <param name="Query">The request query string values.</param>
/// <param name="Body">The request content.</param>
public record RequestLoggingModel(
    [property: JsonPropertyName("method")] string Method,
    [property: JsonPropertyName("path")] string? Url,
    [property: JsonPropertyName("headers")] Dictionary<string, IEnumerable<string?>> Headers,
    [property: JsonPropertyName("query")] Dictionary<string, IEnumerable<string?>> Query,
    [property: JsonPropertyName("body")] string? Body);