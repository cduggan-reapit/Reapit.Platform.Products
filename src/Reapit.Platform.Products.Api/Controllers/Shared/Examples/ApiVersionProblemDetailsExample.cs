using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Shared.Examples;

/// <summary>Example provider for <see cref="ProblemDetails"/> object representing a missing resource.</summary>
[ExcludeFromCodeCoverage]
public class ApiVersionProblemDetailsExample : IExamplesProvider<ProblemDetails>
{
    /// <inheritdoc/>
    public ProblemDetails GetExamples()
        => new()
        {
            Type = "https://docs.api-versioning.org/problems#unspecified",
            Detail = "An API version is required, but was not specified.",
            Title = "Unspecified API version",
            Status = 400
        };
}