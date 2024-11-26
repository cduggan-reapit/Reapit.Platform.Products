using System.Text.Json;

namespace Reapit.Platform.Products.Core.Extensions;

/// <summary> Extension methods for <see cref="string"/> objects.</summary>
public static class StringExtensions
{
    /// <summary>Deserializes a JSON string to an instance of <typeparamref name="T"/> using default settings.</summary>
    /// <param name="input">The string to deserialize.</param>
    /// <typeparam name="T">The type of entity to deserialize the string to.</typeparam>
    /// <returns>The serialized input, or a representation of an empty object (<c>{}</c>) if the input is null.</returns>
    public static T DeserializeTo<T>(this string input)
        => input.DeserializeTo<T>(new JsonSerializerOptions());
    
    /// <summary>Deserializes a JSON string to an instance of <typeparamref name="T"/> using configured settings.</summary>
    /// <param name="input">The string to deserialize.</param>
    /// <param name="options">The serializer options to apply.</param>
    /// <typeparam name="T">The type of entity to deserialize the string to.</typeparam>
    /// <returns>The serialized input, or a representation of an empty object (<c>{}</c>) if the input is null.</returns>
    public static T DeserializeTo<T>(this string input, JsonSerializerOptions options)
        => JsonSerializer.Deserialize<T>(input, options)!;
}