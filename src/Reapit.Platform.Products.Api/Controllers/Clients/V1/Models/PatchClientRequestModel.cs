using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Controllers.Clients.V1.Models;

/// <summary>Definition of the client properties to modify.</summary>
/// <param name="Name">The name of the client.</param>
/// <param name="Description">A description of the client.</param>
/// <param name="LoginUrl">The login uri of your application (not supported by machine to machine clients).</param>
/// <param name="CallbackUrls">Collection of URLs whitelisted for use as a callback after authentication (not supported by machine to machine clients).</param>
/// <param name="SignOutUrls">Collection of URLs which are valid for redirect after logout (not supported by machine to machine clients).</param>
public record PatchClientRequestModel(
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("loginUrl")] string? LoginUrl,
    [property: JsonPropertyName("callbackUrls")] ICollection<string>? CallbackUrls,
    [property: JsonPropertyName("signOutUrls")] ICollection<string>? SignOutUrls);