using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Products.Api.Controllers.Grants.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Grants.V1.Examples;

/// <summary>Example provider for the <see cref="PatchGrantRequestModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class PatchGrantRequestModelExample : IExamplesProvider<PatchGrantRequestModel>
{
    /// <inheritdoc />
    public PatchGrantRequestModel GetExamples()
        => new(["test.read", "test.write", "test.admin"]);
}