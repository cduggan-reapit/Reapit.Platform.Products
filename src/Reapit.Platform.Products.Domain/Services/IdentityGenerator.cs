namespace Reapit.Platform.Products.Domain.Services;

/// <summary>Class responsible for generating new object identifiers.</summary>
public static class IdentityGenerator
{
    /// <summary>Create a new identifier.</summary>
    /// <returns>A 32-character string representing a globally unique identifier (GUID).</returns>
    public static string Create() => Guid.NewGuid().ToString("N");
}