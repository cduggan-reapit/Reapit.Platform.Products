using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Controllers.Clients.V1.Models;

/// <summary>Lightweight representation of a client.</summary>
/// <param name="Id">The unique identifier of the client.</param>
/// <param name="AppId">The unique identifier of the application with which the client is associated.</param>
/// <param name="Type">The type of the client.</param>
/// <param name="Name">The name of the client.</param>
/// <param name="DateCreated">The timestamp of the creation of the client.</param>
/// <param name="DateModified">The timestamp of the last modification to the client.</param>
public record ClientModel(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("applicationId")] string AppId,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("created")] DateTime DateCreated,
    [property: JsonPropertyName("modified")] DateTime DateModified);
