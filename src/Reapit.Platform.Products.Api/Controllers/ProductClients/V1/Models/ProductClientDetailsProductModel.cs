using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Controllers.ProductClients.V1.Models;

/// <summary>Lightweight representation of a product associated with a product client.</summary>
/// <param name="Id">The unique identifier of the product.</param>
/// <param name="Id">The name of the product.</param>
public record ProductClientDetailsProductModel(
    [property: JsonPropertyName("id")] string Id, 
    [property: JsonPropertyName("name")] string Name);