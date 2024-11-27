using MediatR;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UseCases.Products.GetProductById;

/// <summary>Request to get a single product.</summary>
/// <param name="Id">The unique identifier of the product.</param>
public record GetProductByIdQuery(string Id) : IRequest<Product>;