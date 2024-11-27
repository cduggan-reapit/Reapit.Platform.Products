using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UseCases.Products.DeleteProduct;

/// <summary>Request to soft-delete a product.</summary>
/// <param name="Id">The unique identifier of the product.</param>
public record SoftDeleteProductCommand(string Id) : IRequest<Product>;