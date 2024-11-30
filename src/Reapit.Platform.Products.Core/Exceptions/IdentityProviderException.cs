using Microsoft.AspNetCore.Mvc;

namespace Reapit.Platform.Products.Core.Exceptions;

/// <summary>Represents an error raised by the IdP service.</summary>
public class IdentityProviderException : Exception
{
    internal const int ProblemDetailsStatusCode = 500;
    internal const string ProblemDetailsType = "https://www.reapit.com/errors/idp-error";
    internal const string ProblemDetailsTitle = "Identity Provider Exception";
    
    /// <summary>Initializes a new instance of the <see cref="IdentityProviderException"/> class.</summary>
    public IdentityProviderException() : base()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="IdentityProviderException"/> class.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public IdentityProviderException(string message) 
        : base(message)
    {
    }
    
    /// <summary>Initializes a new instance of the <see cref="IdentityProviderException"/> class.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public IdentityProviderException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    /// <summary>Creates a new instance of <see cref="IdentityProviderException"/> representing a null response.</summary>
    /// <remarks>
    /// We don't expect this to get raised in the wild, as auth0 should either throw its own exceptions or return a value,
    /// but since all their endpoints seem to return IThing?, we need to account for that.
    /// </remarks>
    public static IdentityProviderException NullResponse => new("Null response.");
    
    /// <summary>Get an instance of <see cref="ProblemDetails"/> representing an Exception of type <see cref="IdentityProviderException"/>.</summary>
    /// <param name="exception">The exception.</param>
    /// <exception cref="Exception">the exception is not an instance of ValidationException.</exception>
    public static ProblemDetails CreateProblemDetails(Exception exception)
    {
        if (exception is not IdentityProviderException idpException)
            throw ProblemDetailsFactoryException.IncorrectExceptionMapping(exception, typeof(IdentityProviderException));

        return new ProblemDetails
        {
            Status = ProblemDetailsStatusCode,
            Title = ProblemDetailsTitle,
            Type = ProblemDetailsType,
            Detail = idpException.Message
        };
    }
}