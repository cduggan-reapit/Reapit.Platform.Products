using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Products.Api.Controllers.Grants.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Grants.V1.Examples;

/// <summary>Example provider for the <see cref="CreateGrantRequestModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class CreateGrantRequestModelExample : IExamplesProvider<CreateGrantRequestModel>
{
    /// <inheritdoc />
    public CreateGrantRequestModel GetExamples()
        => new("ae8871caaf45435ba16a05a1dff09821", "215fffb81bc24be2a7812cacf7994485", ["test.read", "test.write", "test.admin"]);
}