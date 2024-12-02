using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Controllers.ResourceServers.V1.Models;

/// <summary>Lightweight representation of a resource server.</summary>
/// <param name="Id">The unique identifier of the resource server.</param>
/// <param name="Name">The name of the resource server.</param>
/// <param name="Audience">The audience of the resource server.</param>
/// <param name="DateCreated">The timestamp of the resource servers creation.</param>
/// <param name="DateModified">The timestamp of the last modification to the resource server.</param>
public record ResourceServerModel(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("audience")] string Audience,
    [property: JsonPropertyName("created")] DateTime DateCreated, 
    [property: JsonPropertyName("modified")] DateTime DateModified);