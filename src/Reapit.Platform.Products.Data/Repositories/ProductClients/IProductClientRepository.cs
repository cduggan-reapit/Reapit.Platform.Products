using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Data.Repositories.ProductClients;

/// <summary>Repository for <see cref="Product"/> entities.</summary>

public interface IProductClientRepository : IBaseRepository<ProductClient>
{
    /// <summary>Get a collection of records with optional filters applied.</summary>
    /// <param name="name">Limit results to records with this name.</param>
    /// <param name="description">Limit results to records with a description containing this value.</param>
    /// <param name="productId">Limit results to records associated with the product with this identifier.</param>
    /// <param name="clientId">Limit results to records matching this IdP client identifier.</param>
    /// <param name="grantId">Limit results to records matching this IdP client grant identifier.</param>
    /// <param name="type">Limit results to records with this type.</param>
    /// <param name="pagination">Limit results to a page of records matching this pagination filter.</param>
    /// <param name="dateFilter">Limit results to records matching this date filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of <see cref="Product"/> objects.</returns>
    public Task<IEnumerable<ProductClient>> GetProductClientsAsync(
        string? name = null,
        string? description = null,
        string? productId = null,
        string? clientId = null, 
        string? grantId = null,
        ClientType? type = null,
        PaginationFilter? pagination = null,
        TimestampFilter? dateFilter = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>Get a record by its unique identifier.</summary>
    /// <param name="id">The unique identifier of the record.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The record matching `id` if located, otherwise null.</returns>
    public Task<ProductClient?> GetProductClientByIdAsync(string id, CancellationToken cancellationToken);
}