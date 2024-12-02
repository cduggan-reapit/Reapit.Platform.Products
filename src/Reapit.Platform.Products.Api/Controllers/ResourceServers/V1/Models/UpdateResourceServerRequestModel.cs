using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Controllers.ResourceServers.V1.Models;

/// <summary>Definition of the resource server properties to modify.</summary>
/// <param name="Name">The name of the resource server.</param>
/// <param name="TokenLifetime">Expiration time (in seconds) for access tokens issued for this resource server.</param>
/// <param name="Scopes">Collection of scopes that the resource server uses.</param>
public record UpdateResourceServerRequestModel(
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("tokenLifetime")] int? TokenLifetime,
    [property: JsonPropertyName("scopes")] ICollection<ResourceServerScopeModel>? Scopes);