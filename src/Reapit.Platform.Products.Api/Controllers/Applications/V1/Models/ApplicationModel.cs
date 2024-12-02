using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Controllers.Applications.V1.Models;

/// <summary>Lightweight representation of an application.</summary>
/// <param name="Id">The unique identifier of the application.</param>
/// <param name="Name">The name of the application.</param>
/// <param name="IsFirstParty">Flag indicating whether the application is a first-party application.</param>
/// <param name="DateCreated">The timestamp of the creation of the application.</param>
/// <param name="DateModified">The timestamp of the last modification to the application.</param>
public record ApplicationModel(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("firstParty")] bool IsFirstParty,
    [property: JsonPropertyName("created")] DateTime DateCreated,
    [property: JsonPropertyName("modified")] DateTime DateModified);