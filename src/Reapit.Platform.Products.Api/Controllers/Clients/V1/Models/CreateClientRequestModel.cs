using System.Text.Json.Serialization;
using Reapit.Platform.Products.Domain.Entities.Enums;
using Reapit.Platform.Swagger.Attributes;

namespace Reapit.Platform.Products.Api.Controllers.Clients.V1.Models;

/// <summary>Definition of the client to create.</summary>
/// <param name="AppId">The unique identifier of the application with which the client is associated.</param>
/// <param name="Type">The type of client to create (machine, authCode).</param>
/// <param name="Name">The name of the client.</param>
/// <param name="Description">A description of the client.</param>
/// <param name="LoginUrl">The login uri of your application (not supported by machine to machine clients).</param>
/// <param name="CallbackUrls">Collection of URLs whitelisted for use as a callback after authentication (not supported by machine to machine clients).</param>
/// <param name="SignOutUrls">Collection of URLs which are valid for redirect after logout (not supported by machine to machine clients).</param>
public record CreateClientRequestModel(
    [property: JsonPropertyName("applicationId")] string AppId,
    [property: JsonPropertyName("type"), SwaggerSelect(typeof(ClientType), false)] string Type,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("loginUrl")] string? LoginUrl,
    [property: JsonPropertyName("callbackUrls")] ICollection<string>? CallbackUrls,
    [property: JsonPropertyName("signOutUrls")] ICollection<string>? SignOutUrls);