using System.Text.Json.Serialization;

namespace ProductsCdk.Models;

/// <summary>Context model representing the environment name.</summary>
/// <param name="ShortName">The short environment name (e.g. <c>Dev</c>).</param>
/// <param name="FullName">The full environment name (e.g. <c>Development</c>).</param>
public record EnvironmentContext(
    [property: JsonPropertyName("fullName")] string FullName, 
    [property: JsonPropertyName("shortName")] string ShortName);