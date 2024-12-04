using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Controllers.Clients.V1.Models;

/// <summary>Lightweight representation of a client.</summary>
/// <param name="Id">The unique identifier of the client.</param>
/// <param name="AppId">The unique identifier of the application with which the client is associated.</param>
/// <param name="Type">The type of the client.</param>
/// <param name="Name">The name of the client.</param>
/// <param name="Description">A description of the client.</param>
/// <param name="LoginUrl">The login uri of your application (not supported by machine to machine clients).</param>
/// <param name="CallbackUrls">Collection of URLs whitelisted for use as a callback after authentication (not supported by machine to machine clients).</param>
/// <param name="SignOutUrls">Collection of URLs which are valid for redirect after logout (not supported by machine to machine clients).</param>
/// <param name="DateCreated">The timestamp of the creation of the client.</param>
/// <param name="DateModified">The timestamp of the last modification to the client.</param>
/// <param name="Grants">Lightweight representation of the grants associated with this client.</param>
public record ClientDetailsModel(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("applicationId")] string AppId,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("loginUrl")] string? LoginUrl,
    [property: JsonPropertyName("callbackUrls")] ICollection<string>? CallbackUrls,
    [property: JsonPropertyName("signOutUrls")] ICollection<string>? SignOutUrls,
    [property: JsonPropertyName("created")] DateTime DateCreated,
    [property: JsonPropertyName("modified")] DateTime DateModified,
    [property: JsonPropertyName("grants")] IEnumerable<ClientGrantModel> Grants);
