namespace Reapit.Platform.Products.Core.UseCases;

/// <summary>Common validation messages.</summary>
internal static class CommonValidationMessages
{
    internal const string Required = "Must not be null or empty.";
    
    internal const string Unique = "Must be unique.";
    
    internal const string PageSizeOutOfRange = "Must be between 1 and 100.";

    internal const string CursorOutOfRange = "Must be greater than or equal to zero.";

    internal const string NotEmpty = "Must not be empty.";
}