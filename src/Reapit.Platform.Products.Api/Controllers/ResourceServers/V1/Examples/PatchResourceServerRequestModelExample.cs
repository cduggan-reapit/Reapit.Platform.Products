using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Products.Api.Controllers.ResourceServers.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.ResourceServers.V1.Examples;

/// <summary>Example provider for the <see cref="PatchResourceServerRequestModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class PatchResourceServerRequestModelExample : IExamplesProvider<PatchResourceServerRequestModel>
{
    /// <inheritdoc />
    public PatchResourceServerRequestModel GetExamples()
        => new("Updated API", 43_200, [new ResourceServerScopeModelExample().GetExamples()]);
}