using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Controllers.Grants.V1.Models;

/// <summary>Lightweight representation of a resource server used in the grant model.</summary>
/// <param name="Id">The unique identifier of the resource server.</param>
/// <param name="Name">The unique identifier of the resource server.</param>
public record GrantResourceServerModel(
    [property: JsonPropertyName("id")] string Id, 
    [property: JsonPropertyName("name")] string Name);