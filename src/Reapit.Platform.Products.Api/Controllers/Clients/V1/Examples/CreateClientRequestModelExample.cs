using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Products.Api.Controllers.Clients.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Clients.V1.Examples;

/// <summary>Example provider for the <see cref="CreateClientRequestModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class CreateClientRequestModelExample : IExamplesProvider<CreateClientRequestModel>
{
    /// <inheritdoc />
    public CreateClientRequestModel GetExamples()
        => new(
            AppId: "30f12a25d724416bad9adde96473ab58",
            Type: "authCode",
            Name: "My AuthCode Client",
            Description: "A longer description of the My AuthCode Client.",
            LoginUrl: "https://www.example.net/login",
            CallbackUrls: ["https://www.example.net/post-auth"],
            SignOutUrls: ["https://www.example.net/logout"]);
}