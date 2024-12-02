using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Products.Api.Controllers.ResourceServers.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.ResourceServers.V1.Examples;

/// <summary>Example provider for a page of <see cref="ResourceServerModel"/> objects.</summary>
[ExcludeFromCodeCoverage]
public class ResourceServerModelPageExample : IExamplesProvider<ResultPage<ResourceServerModel>>
{
    /// <inheritdoc />
    public ResultPage<ResourceServerModel> GetExamples()
        => new(
            Data: [new ResourceServerModelExample().GetExamples()], 
            Count: 1, 
            Cursor: 1733129312123456L);
}