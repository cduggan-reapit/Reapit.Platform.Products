using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Controllers.Clients.V1.Models;

/// <summary>Lightweight representation of a grant included in client details response model.</summary>
/// <param name="Id">The unique identifier of the grant.</param>
/// <param name="ResourceServerId">The unique identifier of the resource server with which the grant is associated.</param>
/// <param name="ResourceServerName">The name of the resource server with which the grant is associated.</param>
public record ClientGrantModel(
    [property: JsonPropertyName("id")] string Id, 
    [property: JsonPropertyName("resourceServerId")] string ResourceServerId,
    [property: JsonPropertyName("resourceServer")] string? ResourceServerName);