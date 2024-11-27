using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UseCases.Products.GetProducts;

/// <summary>Request for a collection of products.</summary>
/// <param name="Cursor">The offset cursor; default 0.</param>
/// <param name="PageSize">The maximum number of results to return; default 25.</param>
/// <param name="Name">Limit results to records matching this name.</param>
/// <param name="Description">Limit results to records containing this description.</param>
/// <param name="CreatedFrom">Limit results to records created on or after this date (UTC).</param>
/// <param name="CreatedTo">Limit results to records created before this date (UTC).</param>
/// <param name="ModifiedFrom">Limit results to records last modified on or after this date (UTC).</param>
/// <param name="ModifiedTo">Limit results to records last modified before this date (UTC).</param>
public record GetProductsQuery(
    long? Cursor = null,
    int PageSize = 25,
    string? Name = null,
    string? Description = null,
    DateTime? CreatedFrom = null,
    DateTime? CreatedTo = null,
    DateTime? ModifiedFrom = null,
    DateTime? ModifiedTo = null) 
    : IRequest<IEnumerable<Product>>;