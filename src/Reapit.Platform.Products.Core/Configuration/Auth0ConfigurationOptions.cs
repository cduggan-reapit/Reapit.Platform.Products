using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Core.Configuration;

public class Auth0ConfigurationOptions
{
    /// <summary>The Auth0 domain.</summary>
    [JsonPropertyName("domain")] 
    public required string Domain { get; init; }

    /// <summary>The client connection client Id.</summary>
    [JsonPropertyName("clientId")]
    public required string ClientId { get; init; }

    /// <summary>The client connection client secret.</summary>
    [JsonPropertyName("clientSecret")]
    public required string ClientSecret { get; init; }
    
    /// <summary>How long each management API client should remain alive, in seconds.</summary>
    [JsonPropertyName("tokenTtl")]
    public required int TokenTimeToLive { get; init; }
    
    /// <summary>The audience to declare for client credentials flow apps.</summary>
    [JsonPropertyName("clientGrantAudience")]
    public required string ClientGrantAudience { get; init; }
}