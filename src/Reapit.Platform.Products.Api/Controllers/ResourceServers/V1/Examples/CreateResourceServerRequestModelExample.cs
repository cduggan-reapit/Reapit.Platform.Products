using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Products.Api.Controllers.ResourceServers.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.ResourceServers.V1.Examples;

/// <summary>Example provider for the <see cref="CreateResourceServerRequestModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class CreateResourceServerRequestModelExample : IExamplesProvider<CreateResourceServerRequestModel>
{
    /// <inheritdoc />
    public CreateResourceServerRequestModel GetExamples()
        => new(
            Name: "New API",
            Audience: "https://example.net/audience",
            TokenLifetime: 86_400,
            Scopes: [new ResourceServerScopeModelExample().GetExamples()]);
}