using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Products.Api.Controllers.Clients.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Clients.V1.Examples;

/// <summary>Example provider for a collection of <see cref="ClientModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class ClientModelCollectionExample : IExamplesProvider<ResultPage<ClientModel>>
{
    /// <inheritdoc />
    public ResultPage<ClientModel> GetExamples()
        => new(
            Data: [new ClientModelExample().GetExamples()],
            Cursor: 1733240652511123L,
            Count: 1);
}