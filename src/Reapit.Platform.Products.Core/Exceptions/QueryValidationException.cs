using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Reapit.Platform.Products.Core.Exceptions;

/// <summary>Represents an error caused by request parameters which fail validation checks.</summary>
public class QueryValidationException : Exception
{
    internal const int ProblemDetailsStatusCode = 400;
    internal const string ProblemDetailsType = "https://www.reapit.com/errors/bad-request";
    internal const string ProblemDetailsTitle = "Bad Request";

    /// <summary>The collection of validation failures represented by this exception.</summary>
    public IEnumerable<ValidationFailure> Errors { get; private init; } = new List<ValidationFailure>();
    
    /// <summary>Initializes a new instance of the <see cref="QueryValidationException"/> class.</summary>
    public QueryValidationException() : base()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="QueryValidationException"/> class.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public QueryValidationException(string message) 
        : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="QueryValidationException"/> class.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public QueryValidationException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    /// <summary>Gets an instance of <see cref="QueryValidationException"/> representing a failed <see cref="ValidationResult"/>.</summary>
    /// <param name="result">The validation result.</param>
    public static QueryValidationException ValidationFailed(ValidationResult result) 
        => new("One or more validation errors occurred.") { Errors = result.Errors };

    /// <summary>Get an instance of <see cref="ProblemDetails"/> representing an Exception of type <see cref="QueryValidationException"/>.</summary>
    /// <param name="exception">The exception.</param>
    /// <exception cref="Exception">the exception is not an instance of ValidationException.</exception>
    public static ProblemDetails CreateProblemDetails(Exception exception)
    {
        if (exception is not QueryValidationException badRequestException)
            throw ProblemDetailsFactoryException.IncorrectExceptionMapping(exception, typeof(QueryValidationException));

        var problemDetails = new ProblemDetails
        {
            Title = ProblemDetailsTitle,
            Type = ProblemDetailsType,
            Detail = badRequestException.Message,
            Status = ProblemDetailsStatusCode,
        };

        if (!badRequestException.Errors.Any())
            return problemDetails;

        problemDetails.Extensions.Add("errors", badRequestException.Errors.GroupBy(e => e.PropertyName)
            .ToDictionary(
                keySelector: group => System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(group.Key),
                elementSelector: group => group.Select(item => item.ErrorMessage)));

        return problemDetails;
    }
}