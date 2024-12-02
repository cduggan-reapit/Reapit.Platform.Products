using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Products.Api.Controllers.Applications.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Applications.V1.Examples;

/// <summary>Example provider for the <see cref="PatchApplicationRequestModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class PatchApplicationRequestModelExample : IExamplesProvider<PatchApplicationRequestModel>
{
    /// <inheritdoc />
    public PatchApplicationRequestModel GetExamples()
        => new("Updated Example Application", "An updated example application with two clients.");
}