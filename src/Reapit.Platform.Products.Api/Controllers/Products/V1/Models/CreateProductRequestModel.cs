using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Controllers.Products.V1.Models;

/// <summary>Request model used when creating a new product.</summary>
/// <param name="Name">The name of the product.</param>
/// <param name="Description">A description of the product.</param>
public record CreateProductRequestModel(
    [property: JsonPropertyName("name")] string Name, 
    [property: JsonPropertyName("description")] string? Description);