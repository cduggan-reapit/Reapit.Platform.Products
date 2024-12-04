using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Controllers.Grants.V1.Models;

/// <summary>Lightweight representation of a client used in the grant model.</summary>
/// <param name="Id">The unique identifier of the client.</param>
/// <param name="Name">The name of the client.</param>
/// <param name="Type">The type of the client.</param>
public record GrantClientModel(
    [property: JsonPropertyName("id")] string Id, 
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("type")] string Type);