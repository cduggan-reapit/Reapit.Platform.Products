using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Controllers.Applications.V1.Models;

/// <summary>Model defining application properties to patch.</summary>
/// <param name="Name">The name of the application.</param>
/// <param name="Description">An optional description of the application.</param>
public record PatchApplicationRequestModel(
    [property: JsonPropertyName("name")] string? Name, 
    [property: JsonPropertyName("description")] string? Description);