using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Products.Api.Controllers.Grants.V1.Models;
using Reapit.Platform.Products.Domain.Entities.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Grants.V1.Examples;

/// <summary>Example provider for the <see cref="GrantModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class GrantModelExample : IExamplesProvider<GrantModel>
{
    private static readonly DateTime BaseDate = new(2024, 12, 4, 8, 32, 47, DateTimeKind.Utc);

    /// <inheritdoc />
    public GrantModel GetExamples()
        => new(Id: "595746e40ca747bcb1804b2cae8fcd68",
            Client: new GrantClientModel("ae8871caaf45435ba16a05a1dff09821", "My AuthCode Client", ClientType.Machine.Name),
            ResourceServer: new GrantResourceServerModel("215fffb81bc24be2a7812cacf7994485", "My Product"),
            Scopes: ["test.read","test.write","test.admin"],
            DateCreated: BaseDate,
            DateModified: BaseDate.AddDays(3).AddHours(2).AddMinutes(17));
}