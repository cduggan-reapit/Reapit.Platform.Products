using Reapit.Platform.Products.Api.Controllers.ProductClients.V1.Models;
using Reapit.Platform.Products.Domain.Entities.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.Products.Api.Controllers.ProductClients.V1.Examples;

/// <summary>Examples provider for the <see cref="CreateProductClientRequestModel"/> type.</summary>
public class CreateProductClientRequestModelExampleProvider : IExamplesProvider<CreateProductClientRequestModel>
{
    /// <inheritdoc />
    public CreateProductClientRequestModel GetExamples()
        => new (
            ProductId: "62b650b9769f4cc4b9638dc720ddfa1f",
            Name: "My Product AuthCode Client",
            Description: "AuthCode client for the My Product product.",
            Type: ClientType.AuthorizationCode.Name,
            Audience: null,
            CallbackUrls: ["https://example.net/callback"],
            SignOutUrls: ["https://example.net/sign-out"]);
}