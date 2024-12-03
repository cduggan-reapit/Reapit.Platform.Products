using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Products.Api.Controllers.Clients.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Clients.V1.Examples;

/// <summary>Example provider for the <see cref="ClientModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class ClientModelExample : IExamplesProvider<ClientModel>
{
    private static readonly DateTime BaseDate = new(2024, 12, 3, 15, 44, 17, DateTimeKind.Utc);

    /// <inheritdoc />
    public ClientModel GetExamples()
        => new(
            Id: "63f418a1216f4fd1a770a0c5fa189ac0",
            AppId: "9cb04d6d77874739990e24f17b85c5f8",
            Name: "My AuthCode Client",
            Type: "authCode",
            DateCreated: BaseDate,
            DateModified: BaseDate.AddMonths(1).AddHours(1).AddMinutes(12).AddSeconds(3));
}