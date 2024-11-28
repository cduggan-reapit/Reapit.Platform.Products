using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Controllers.ProductClients.V1.Models;

/// <summary>Detailed representation of a product client.</summary>
/// <param name="Id">The unique identifier of the product client.</param>
/// <param name="Name">The name of the product client.</param>
/// <param name="Description">A description of the product client.</param>
/// <param name="Type">The type of the product client.</param>
/// <param name="Audience">The audience of client grant (client_credentials clients only).</param>
/// <param name="DateCreated">The timestamp of the creation of the product client (UTC).</param>
/// <param name="DateModified">The timestamp of the last modification to the product client (UTC).</param>
/// <param name="CallbackUrls">Collection of URLs whitelisted for use as a callback to the client after authentication.</param>
/// <param name="SignOutUrls">Collection of URLs that are valid to redirect to after logout.</param>
/// <param name="Product">Lightweight representation of the product with which the product client is associated.</param>
public record ProductClientDetailsModel(
    [property: JsonPropertyName("id")] string Id, 
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("audience")] string? Audience,
    [property: JsonPropertyName("created")] DateTime DateCreated, 
    [property: JsonPropertyName("modified")] DateTime DateModified,
    [property: JsonPropertyName("callbackUrls")] IEnumerable<string>? CallbackUrls,
    [property: JsonPropertyName("signOutUrls")] IEnumerable<string>? SignOutUrls,
    [property: JsonPropertyName("product")] ProductClientDetailsProductModel Product);