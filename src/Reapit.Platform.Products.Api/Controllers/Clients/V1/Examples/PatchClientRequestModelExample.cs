using System.Diagnostics.CodeAnalysis;
using Reapit.Platform.Products.Api.Controllers.Clients.V1.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.Clients.V1.Examples;

/// <summary>Example provider for the <see cref="PatchClientRequestModel"/> type.</summary>
[ExcludeFromCodeCoverage]
public class PatchClientRequestModelExample : IExamplesProvider<PatchClientRequestModel>
{
    /// <inheritdoc />
    public PatchClientRequestModel GetExamples()
        => new(Name: "My AuthCode Client",
            Description: "A longer description of the My AuthCode Client.",
            LoginUrl: "https://www.example.net/login",
            CallbackUrls: ["https://www.example.net/post-auth"],
            SignOutUrls: ["https://www.example.net/logout"]);
}