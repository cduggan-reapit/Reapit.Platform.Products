using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Controllers.ResourceServers.V1.Models;

/// <summary>Representation of a resource server scope.</summary>
/// <param name="Value">The value of the scope.</param>
/// <param name="Description">An optional, user-friendly description of the scope.</param>
public record ResourceServerScopeModel(
    [property: JsonPropertyName("value")] string Value, 
    [property: JsonPropertyName("description")] string? Description);