using System.Text.Json;
using System.Text.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Reapit.Platform.Products.Core.Extensions;

/// <summary>Extension methods for generic objects.</summary>
public static class ObjectExtensions
{
    /// <summary>Serializes the object to a JSON string using default settings.</summary>
    /// <param name="input">The object to serialize.</param>
    /// <returns>The serialized input, or a representation of an empty object (<c>{}</c>) if the input is null.</returns>
    public static string ToJson(this object? input)
        => input.ToJson(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never
        });
    
    /// <summary>Serializes the object to a JSON string using configured settings.</summary>
    /// <param name="input">The object to serialize.</param>
    /// <param name="options">The serializer options to apply.</param>
    /// <returns>The serialized input, or a representation of an empty object (<c>{}</c>) if the input is null.</returns>
    public static string ToJson(this object? input, JsonSerializerOptions options)
        => JsonSerializer.Serialize(input ?? new { }, options);
}