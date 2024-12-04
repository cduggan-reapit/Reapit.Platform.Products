namespace Reapit.Platform.Products.Core.UseCases.Grants;

/// <summary>Grant validation messages.</summary>
public static class GrantValidationMessages
{
    public const string ClientNotFound = "Invalid client identifier";
    public const string ResourceServerNotFound = "Invalid resource server identifier";
    public const string UnsupportedScope = "Scope not supported";
}