using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Infrastructure.Configuration;

/// <summary>Representation of the service configuration database object.</summary>
public record DatabaseConfigurationModel(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("user")] string UserSecretPath);