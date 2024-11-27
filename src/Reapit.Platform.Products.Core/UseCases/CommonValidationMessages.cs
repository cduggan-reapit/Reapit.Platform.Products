namespace Reapit.Platform.Products.Core.UseCases;

/// <summary>Common validation messages.</summary>
public static class CommonValidationMessages
{
    public const string Required = "Must not be null or empty.";
    
    public const string Unique = "Must be unique.";
    
    internal const string PageSizeOutOfRange = "Must be between 1 and 100.";

    internal const string CursorOutOfRange = "Must be greater than or equal to zero.";
}