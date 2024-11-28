using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UseCases.ProductClients.PatchProductClient;

/// <summary>Request to update a product client.</summary>
/// <param name="Id">The unique identifier of the product client.</param>
/// <param name="Name">The name of the product client.</param>
/// <param name="Description">A description of the product client.</param>
/// <param name="CallbackUrls">Collection of URLs whitelisted for use as a callback to the client after authentication.</param>
/// <param name="SignOutUrls">Collection of URLs that are valid to redirect to after logout.</param>
public record PatchProductClientCommand(
    string Id, 
    string? Name, 
    string? Description, 
    ICollection<string>? CallbackUrls,
    ICollection<string>? SignOutUrls)
    : IRequest<ProductClient>;