using Reapit.Platform.Products.Api.Controllers.ProductClients.V1.Models;
using Reapit.Platform.Products.Domain.Entities.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.ProductClients.V1.Examples;

/// <summary>Examples provider for the <see cref="PatchProductClientRequestModel"/> type.</summary>
public class PatchProductClientRequestModelExampleProvider : IExamplesProvider<PatchProductClientRequestModel>
{
    /// <inheritdoc />
    public PatchProductClientRequestModel GetExamples()
        => new (
            Name: "My Product AuthCode Client",
            Description: "AuthCode client for the My Product product.",
            CallbackUrls: ["https://example.net/callback"],
            SignOutUrls: ["https://example.net/sign-out"]);
}