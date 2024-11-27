using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UseCases.ProductClients.DeleteProductClient;

/// <summary>Request to delete a product client.</summary>
/// <param name="Id">The unique identifier of the product client.</param>
public record SoftDeleteProductClientCommand(string Id) : IRequest<ProductClient>;