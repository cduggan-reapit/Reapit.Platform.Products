namespace Reapit.Platform.Products.Core.UseCases.ProductClients;

/// <summary>Validation messages associated with product clients.</summary>
public static class ProductClientValidationMessages
{
    internal const string NameTooLong = "Exceeds maximum length of 100 characters.";
    
    internal const string NameMalformed = "Must not contain chevron characters (`<`, `>`).";
    
    internal const string DescriptionTooLong = "Exceeds maximum length of 140 characters.";

    internal const string ProductNotFound = "Invalid product identifier.";
    
    internal const string UnsupportedType = "Unsupported type.";

    internal const string UnsupportedByClientCredentials = "Not supported by client_credentials clients";
    
    internal const string RequiredByClientCredentials = "Required for client_credentials clients";
    
    internal const string UnsupportedByAuthorizationCode = "Not supported by authorization_code clients";
    
    internal const string RequiredByAuthorizationCode = "Required for authorization_code clients";
}