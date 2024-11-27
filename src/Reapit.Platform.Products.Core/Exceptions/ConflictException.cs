using Microsoft.AspNetCore.Mvc;

namespace Reapit.Platform.Products.Core.Exceptions;

/// <summary>Represents an error caused by resource conflicts.</summary>
public class ConflictException : Exception
{
    internal const int ProblemDetailsStatusCode = 409;
    internal const string ProblemDetailsType = "https://www.reapit.com/errors/conflict";
    internal const string ProblemDetailsTitle = "Resource Conflict";
    
    /// <summary>Initializes a new instance of the <see cref="ConflictException"/> class.</summary>
    public ConflictException() : base()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ConflictException"/> class.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public ConflictException(string message) 
        : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ConflictException"/> class.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ConflictException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
    
    /// <summary>Get an instance of <see cref="ConflictException"/> representing an 'already exists' error. </summary>
    /// <param name="resourceType">The type of resource.</param>
    /// <param name="resourceIdentifier">The resource identifier.</param>
    public static ConflictException ResourceExists(
        string resourceType,
        string resourceIdentifier)
        => new($"{resourceType} already exists for '{resourceIdentifier}'.");
    
    /// <summary>Get an instance of <see cref="ProblemDetails"/> representing an Exception of type <see cref="ConflictException"/>.</summary>
    /// <param name="exception">The exception.</param>
    /// <exception cref="Exception">the exception is not an instance of ConflictException.</exception>
    public static ProblemDetails CreateProblemDetails(Exception exception)
    {
        if (exception is not ConflictException conflictException)
            throw ProblemDetailsFactoryException.IncorrectExceptionMapping(exception, typeof(ConflictException));

        return new ProblemDetails
        {
            Title = ProblemDetailsTitle,
            Type = ProblemDetailsType,
            Status = ProblemDetailsStatusCode,
            Detail = exception.Message
        };
    }
}