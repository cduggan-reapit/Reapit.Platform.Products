using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Controllers.Dummies.V1.Models;

/// <summary>Response model used when fetching one or more Dummy's.</summary>
/// <param name="Id">The unique identifier of the Dummy.</param>
/// <param name="Name">The name of the Dummy.</param>
/// <param name="DateCreated">The date and time (UTC) on which the Dummy was created.</param>
/// <param name="DateModified">The date and time (UTC) on which the Dummy was last modified.</param>
public record ReadDummyResponseModel(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("created")] DateTime DateCreated,
    [property: JsonPropertyName("modified")] DateTime DateModified);