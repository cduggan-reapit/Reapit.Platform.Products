using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UseCases.ProductClients.GetProductClientById;

/// <summary>Request to get a single product client.</summary>
/// <param name="Id">The unique identifier of the product client.</param>
public record GetProductClientByIdQuery(string Id) : IRequest<ProductClient>;