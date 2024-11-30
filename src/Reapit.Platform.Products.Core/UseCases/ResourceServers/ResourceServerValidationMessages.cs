namespace Reapit.Platform.Products.Core.UseCases.ResourceServers;

/// <summary>Validation messages for resource server requests.</summary>
public static class ResourceServerValidationMessages
{
    public const string NameTooLong = "Exceeds maximum length of 200 characters";
    public const string NameInvalid = "Contains forbidden characters ('<', '>').";
    public const string AudienceTooLong = "Exceeds maximum length of 600 characters";
    public const string TokenLifetimeOutOfRange = "Must be between 60 and 86,400 (inclusive).";
}