using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.Repositories.Products;

/// <summary>Repository for <see cref="Product"/> entities.</summary>

public interface IProductRepository : IBaseRepository<Product>
{
    /// <summary>Get a collection of records with optional filters applied.</summary>
    /// <param name="name">Limit results to records with this name.</param>
    /// <param name="description">Limit results to records with a description containing this value.</param>
    /// <param name="pagination">Limit results to a page of records matching this pagination filter.</param>
    /// <param name="dateFilter">Limit results to records matching this date filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of <see cref="Product"/> objects.</returns>
    public Task<IEnumerable<Product>> GetProductsAsync(
        string? name = null,
        string? description = null,
        PaginationFilter? pagination = null,
        TimestampFilter? dateFilter = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>Get a record by its unique identifier.</summary>
    /// <param name="id">The unique identifier of the record.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The record matching `id` if located, otherwise null.</returns>
    public Task<Product?> GetProductByIdAsync(string id, CancellationToken cancellationToken);
}