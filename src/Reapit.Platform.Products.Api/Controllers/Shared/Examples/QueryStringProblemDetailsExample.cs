using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Shared.Examples;

/// <summary>Example provider for <see cref="ProblemDetails"/> object representing a query string validation error.</summary>
[ExcludeFromCodeCoverage]
public class QueryStringProblemDetailsExample : IExamplesProvider<ProblemDetails>
{
    /// <inheritdoc/>
    public ProblemDetails GetExamples()
        => new()
        {
            Type = "https://www.reapit.com/errors/bad-request",
            Detail = "One or more validation errors occurred.",
            Title = "Bad Request",
            Status = 400,
            Extensions =
            {
                {
                    "errors", new Dictionary<string, string[]>
                    {
                        { "propertyName", ["errorMessage"] }
                    }
                }
            }
        };
}