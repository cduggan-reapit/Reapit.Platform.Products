using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Controllers.ResourceServers.V1.Models;

/// <summary>Detailed representation of a resource server.</summary>
/// <param name="Id">The unique identifier of the resource server.</param>
/// <param name="Name">The name of the resource server.</param>
/// <param name="Audience">The audience of the resource server.</param>
/// <param name="TokenLifetime">Expiration time (in seconds) for access tokens issued for the resource server.</param>
/// <param name="Scopes">The scopes that the resource server uses.</param>
/// <param name="DateCreated">The timestamp of the resource servers creation.</param>
/// <param name="DateModified">The timestamp of the last modification to the resource server.</param>
public record ResourceServerDetailsModel(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("audience")] string Audience,
    [property: JsonPropertyName("tokenLifetime")] int TokenLifetime,
    [property: JsonPropertyName("scopes")] IEnumerable<ResourceServerScopeModel> Scopes,
    [property: JsonPropertyName("created")] DateTime DateCreated, 
    [property: JsonPropertyName("modified")] DateTime DateModified);