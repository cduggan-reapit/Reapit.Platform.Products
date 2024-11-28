using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Controllers.ProductClients.V1.Models;

/// <summary>Request model used when updating a product client.</summary>
/// <param name="Name">The name of the product client.</param>
/// <param name="Description">A description of the product client.</param>
/// <param name="CallbackUrls">Collection of URLs whitelisted for use as a callback to the client after authentication.</param>
/// <param name="SignOutUrls">Collection of URLs that are valid to redirect to after logout. Wildcards are allowed for subdomains.</param>
public record PatchProductClientRequestModel(
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("callbackUrls")] ICollection<string>? CallbackUrls,
    [property: JsonPropertyName("signOutUrls")] ICollection<string>? SignOutUrls);