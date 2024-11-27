using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Controllers.ProductClients.V1.Models;

/// <summary>Lightweight representation of a product client.</summary>
/// <param name="Id">The unique identifier of the product client.</param>
/// <param name="Name">The name of the product client.</param>
/// <param name="Type">The type of the product client.</param>
/// <param name="ProductId">The unique identifier of the product with which the client is associated.</param>
/// <param name="DateCreated">The timestamp of the creation of the product client (UTC).</param>
/// <param name="DateModified">The timestamp of the last modification to the product client (UTC).</param>
public record ProductClientModel(
    [property: JsonPropertyName("id")] string Id, 
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("productId")] string ProductId,
    [property: JsonPropertyName("created")] DateTime DateCreated, 
    [property: JsonPropertyName("modified")] DateTime DateModified);