using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Products.Api.Controllers.Grants.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Grants.V1.Examples;

/// <summary>Example provider for a page of <see cref="GrantModel"/> objects.</summary>
[ExcludeFromCodeCoverage]
public class GrantModelCollectionExample : IExamplesProvider<ResultPage<GrantModel>>
{
    /// <inheritdoc />
    public ResultPage<GrantModel> GetExamples()
        => new(
            Data: [new GrantModelExample().GetExamples()],
            Cursor: 1733301238823987L,
            Count: 1);
}