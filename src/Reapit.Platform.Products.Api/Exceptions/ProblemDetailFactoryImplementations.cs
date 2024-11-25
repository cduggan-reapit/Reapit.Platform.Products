using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.Common.Exceptions;
using Reapit.Platform.Common.Interfaces;
using static System.Text.Json.JsonNamingPolicy;

namespace Reapit.Platform.Products.Api.Exceptions;

/// <summary>Problem detail factory methods for use with <see cref="IProblemDetailsFactory"/>.</summary>
public static class ProblemDetailFactoryImplementations
{
    /// <summary>Get an instance of <see cref="ProblemDetails"/> representing an Exception of type <see cref="ValidationException"/>.</summary>
    /// <param name="exception">The exception.</param>
    /// <exception cref="Exception">the exception is not an instance of ValidationException.</exception>
    public static ProblemDetails GetValidationExceptionProblemDetails(Exception exception)
    {
        if (exception is not ValidationException validationException)
            throw ProblemDetailsFactoryException.IncorrectExceptionMapping(exception, typeof(ValidationException)); 

        return new ProblemDetails
        {
            Title = "Validation Failed",
            Type = "https://www.reapit.com/errors/validation",
            Detail = "One or more validation errors occurred.",
            Status = 422,
            Extensions =
            {
                {
                    "errors", validationException.Errors.GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            keySelector: group => CamelCase.ConvertName(group.Key),
                            elementSelector: group => group.Select(item => item.ErrorMessage))
                }
            }
        };
    }
}