using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Shared.Examples;

/// <summary>Example provider for <see cref="ProblemDetails"/> object representing a conflicting resource.</summary>
[ExcludeFromCodeCoverage]
public class ConflictProblemDetailsExample : IExamplesProvider<ProblemDetails>
{
    /// <inheritdoc/>
    public ProblemDetails GetExamples()
        => new()
        {
            Type = "https://www.reapit.com/errors/conflict",
            Detail = "Resource already exists with the identifier '...'.",
            Title = "Resource Conflict",
            Status = 409
        };
}