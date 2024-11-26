using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Controllers.Products.V1.Models;

/// <summary>Lightweight representation of a product client used as part of the product detail model.</summary>
/// <param name="Id">The unique identifier of the client.</param>
/// <param name="Name">The name of the client.</param>
/// <param name="Type">The type of client.</param>
public record ProductDetailsClientModel(
    [property: JsonPropertyName("id")] string Id, 
    [property: JsonPropertyName("name")] string Name, 
    [property: JsonPropertyName("type")] string Type);