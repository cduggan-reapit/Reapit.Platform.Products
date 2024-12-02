using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Controllers.Applications.V1.Models;

/// <summary>Lightweight representation of a client associated with an application.</summary>
/// <param name="Id">The unique identifier of the client.</param>
/// <param name="Name">The name of the client.</param>
/// <param name="Type">The type of client.</param>
public record ApplicationClientModel(
    [property: JsonPropertyName("id")] string Id, 
    [property: JsonPropertyName("name")] string Name, 
    [property: JsonPropertyName("type")] string Type);