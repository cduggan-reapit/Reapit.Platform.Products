using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Controllers.Products.V1.Models;

/// <summary>Lightweight representation of a product.</summary>
/// <param name="Id">The unique identifier of the product.</param>
/// <param name="Name">The name of the product.</param>
/// <param name="DateCreated">The timestamp of the creation of the product (UTC).</param>
/// <param name="DateModified">The timestamp of the last modification to the product (UTC).</param>
public record ProductModel(
    [property: JsonPropertyName("id")] string Id, 
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("created")] DateTime DateCreated, 
    [property: JsonPropertyName("modified")] DateTime DateModified);