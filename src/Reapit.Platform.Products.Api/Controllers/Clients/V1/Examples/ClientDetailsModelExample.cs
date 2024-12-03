using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Products.Api.Controllers.Clients.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Clients.V1.Examples;

/// <summary>Example provider for the <see cref="ClientDetailsModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class ClientDetailsModelExample : IExamplesProvider<ClientDetailsModel>
{
    private static readonly DateTime BaseDate = new(2024, 12, 3, 15, 44, 17, DateTimeKind.Utc);
    
    /// <inheritdoc />
    public ClientDetailsModel GetExamples()
        => new(
            Id: "63f418a1216f4fd1a770a0c5fa189ac0",
            AppId: "9cb04d6d77874739990e24f17b85c5f8",
            Name: "My AuthCode Client",
            Description: "A longer description of the My AuthCode Client.",
            Type: "authCode",
            LoginUrl: "https://www.example.net/login",
            CallbackUrls: [ "https://www.example.net/post-auth" ],
            SignOutUrls: [ "https://www.example.net/logout" ],
            DateCreated: BaseDate,
            DateModified: BaseDate.AddMonths(1).AddHours(1).AddMinutes(12).AddSeconds(3),
            Grants: [
                new ClientGrantModel("57fe10122e3041ba8d20fc6ad5a33ca7", "7e2887babd3340fca3373ac48629d0f8", "Agency Cloud"),
                new ClientGrantModel("9f45fbff96b549f199c7d5ac1ae4af67", "5920f61387e94d1ebec2a9dad19c407a", "Access Management")
            ]);
}