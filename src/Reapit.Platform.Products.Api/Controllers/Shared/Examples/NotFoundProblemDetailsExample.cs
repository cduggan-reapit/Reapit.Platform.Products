using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Shared.Examples;


/// <summary>Example provider for <see cref="ProblemDetails"/> object representing a missing resource.</summary>
[ExcludeFromCodeCoverage]
public class NotFoundProblemDetailsExample : IExamplesProvider<ProblemDetails>
{
    /// <inheritdoc/>
    public ProblemDetails GetExamples()
        => new()
        {
            Type = "https://www.reapit.com/platform/errors/not-found",
            Detail = "The requested resource was not found.",
            Title = "Resource Not Found",
            Status = 404
        };
}