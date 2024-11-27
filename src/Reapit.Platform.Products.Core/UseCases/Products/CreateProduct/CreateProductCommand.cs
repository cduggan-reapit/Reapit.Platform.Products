using MediatR;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UseCases.Products.CreateProduct;

/// <summary>Command to create a product.</summary>
/// <param name="Name">The name of the product.</param>
/// <param name="Description">A description of the product.</param>
public record CreateProductCommand(string Name, string? Description) : IRequest<Product>;