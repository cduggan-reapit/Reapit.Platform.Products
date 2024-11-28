using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Reapit.Platform.Products.Core.Extensions;

/// <summary>Extension methods for generic objects.</summary>
public static class ObjectExtensions
{
    /// <summary>Serializes the object to a JSON string using default settings.</summary>
    /// <param name="input">The object to serialize.</param>
    /// <returns>The serialized input, or a representation of an empty object (<c>{}</c>) if the input is null.</returns>
    public static string ToJson(this object? input)
        => JsonSerializer.Serialize(input ?? new { });
}