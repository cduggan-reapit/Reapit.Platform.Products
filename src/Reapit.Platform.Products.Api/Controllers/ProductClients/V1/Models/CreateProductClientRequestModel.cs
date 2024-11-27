using System.Text.Json.Serialization;
using Reapit.Platform.Products.Domain.Entities.Enums;
using Reapit.Platform.Swagger.Attributes;

namespace Reapit.Platform.Products.Api.Controllers.ProductClients.V1.Models;

/// <summary>Request model used when creating a new product client.</summary>
/// <param name="ProductId">The unique identifier of the product to which the client belongs.</param>
/// <param name="Name">The name of the product client.</param>
/// <param name="Description">A description of the product client.</param>
/// <param name="Type">The type of the product client.</param>
/// <param name="CallbackUrls">Collection of URLs whitelisted for use as a callback to the client after authentication.</param>
/// <param name="SignOutUrls">Collection of URLs that are valid to redirect to after logout. Wildcards are allowed for subdomains.</param>
public record CreateProductClientRequestModel(
    [property: JsonPropertyName("productId")] string ProductId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("type"), SwaggerSelect(typeof(ClientType), false)] string Type,
    [property: JsonPropertyName("callbackUrls")] IEnumerable<string>? CallbackUrls,
    [property: JsonPropertyName("signOutUrls")] IEnumerable<string>? SignOutUrls);