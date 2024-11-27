using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UseCases.Products.PatchProduct;

/// <summary>Request to update a product.</summary>
/// <param name="Id">The unique identifier of the product.</param>
/// <param name="Name">The name of the product.</param>
/// <param name="Description">The description of the product.</param>
public record PatchProductCommand(string Id, string? Name, string? Description) : IRequest<Product>;