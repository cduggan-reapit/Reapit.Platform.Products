using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.ProductClients;

internal static class TestProductClientFactory
{
    internal static ProductClient GetProductClient(
        string productId = "productId",
        string clientId = "clientId",
        string grantId = "grantId",
        string name = "name",
        string description = "description",
        ClientType? type = null,
        string? audience = null, 
        ICollection<string>? callbackUrls = null, 
        ICollection<string>? signOutUrls = null)
        => new(productId, clientId, grantId, name, description, type ?? ClientType.ClientCredentials, audience, callbackUrls, signOutUrls);
}