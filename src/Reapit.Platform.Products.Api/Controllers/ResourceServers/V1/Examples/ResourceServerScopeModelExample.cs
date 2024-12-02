using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Products.Api.Controllers.ResourceServers.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.ResourceServers.V1.Examples;

/// <summary>Example provider for the <see cref="ResourceServerScopeModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class ResourceServerScopeModelExample : IExamplesProvider<ResourceServerScopeModel>
{
    /// <inheritdoc />
    public ResourceServerScopeModel GetExamples()
        => new("subject.read", "Read access to subjects.");
}