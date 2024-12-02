using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Products.Api.Controllers.Applications.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Applications.V1.Examples;

/// <summary>Example provider for the <see cref="CreateApplicationRequestModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class CreateApplicationRequestModelExample : IExamplesProvider<CreateApplicationRequestModel>
{
    /// <inheritdoc />
    public CreateApplicationRequestModel GetExamples()
        => new("Custom Application", "An example application.", true);
}