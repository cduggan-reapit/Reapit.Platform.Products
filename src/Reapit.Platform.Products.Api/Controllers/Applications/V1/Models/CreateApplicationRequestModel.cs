using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Controllers.Applications.V1.Models;

/// <summary>Definition of the application to create.</summary>
/// <param name="Name">The name of the application.</param>
/// <param name="Description">An optional description of the application.</param>
/// <param name="IsFirstParty">Flag indicating whether the application is a first-party application.</param>
public record CreateApplicationRequestModel(
    [property: JsonPropertyName("name")] string Name, 
    [property: JsonPropertyName("description")] string? Description, 
    [property: JsonPropertyName("firstParty")] bool IsFirstParty);