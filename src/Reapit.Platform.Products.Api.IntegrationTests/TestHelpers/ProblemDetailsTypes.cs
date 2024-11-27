namespace Reapit.Platform.Products.Api.IntegrationTests.TestHelpers;

/// <summary>
/// These are centralised in case a package version change alters the text - we don't want to have 9000 lines to update.
/// </summary>
public static class ProblemDetailsTypes
{
    public const string UnsupportedApiVersion = "Unsupported API version";

    public const string UnspecifiedApiVersion = "Unspecified API version";

    public const string ResourceNotFound = "Resource Not Found";

    public const string ValidationFailed = "Validation Failed";
    
    public const string QueryStringInvalid = "Bad Request";

    public const string ResourceConflict = "Resource Conflict";
}