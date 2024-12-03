namespace Reapit.Platform.Products.Core.UseCases.Clients;

/// <summary>Validation messages associated with clients.</summary>
public static class ClientValidationMessages
{
    public const string NameTooLong = "Exceeds maximum length of 100 characters";
    public const string NameInvalid = "Contains forbidden characters ('<', '>')";
    public const string TypeInvalid = "Client type not recognised";
    public const string DescriptionTooLong = "Exceeds maximum length of 140 characters";
    public const string ApplicationNotFound = "Invalid application identifier";
    public const string LoginUrlMustBeHttps = "Login URL must be a secure address";

    public const string NotSupportedByMachineClients = "Not supported by machine clients";
    public const string RequiredByAuthCodeClients = "Required for authCode clients";
}