using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UseCases.ProductClients.CreateProductClient;

/// <summary>Request to create a product client.</summary>
/// <param name="ProductId">The unique identifier of the product with which the client is associated.</param>
/// <param name="Name">The name of the product client.</param>
/// <param name="Description">A description of the product client.</param>
/// <param name="Type">The type of client.</param>
/// <param name="Audience">The audience of client grant.</param>
/// <param name="CallbackUrls">Collection of URLs whitelisted for use as a callback to the client after authentication.</param>
/// <param name="SignOutUrls">Collection of URLs that are valid to redirect to after logout.</param>
public record CreateProductClientCommand(
    string ProductId, 
    string Name, 
    string? Description, 
    string Type,
    string? Audience,
    ICollection<string>? CallbackUrls,
    ICollection<string>? SignOutUrls)
    : IRequest<ProductClient>;