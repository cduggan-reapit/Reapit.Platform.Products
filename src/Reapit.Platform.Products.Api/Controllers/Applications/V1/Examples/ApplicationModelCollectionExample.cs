using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Products.Api.Controllers.Applications.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Applications.V1.Examples;

/// <summary>Example provider for a page of <see cref="ApplicationModel"/> objects.</summary>
[ExcludeFromCodeCoverage]
public class ApplicationModelCollectionExample : IExamplesProvider<ResultPage<ApplicationModel>>
{
    /// <inheritdoc />
    public ResultPage<ApplicationModel> GetExamples()
        => new(
            Data: [new ApplicationModelExample().GetExamples()], 
            Count: 1,
            Cursor: 1733129312123456L);
}