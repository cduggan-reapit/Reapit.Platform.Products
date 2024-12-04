using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.Repositories.Grants;

/// <summary>Repository service for the <see cref="Grant"/> type.</summary>
public interface IGrantRepository : IBaseRepository<Grant>
{
    /// <summary>Get a collection of grants with optional filters applied.</summary>
    /// <param name="clientId">Limit results to records associated with the client with this unique identifier.</param>
    /// <param name="resourceServerId">Limit results to records associated with the api with this unique identifier.</param>
    /// <param name="pagination">Limit results to a page matching this pagination filter.</param>
    /// <param name="dateFilter">Limit results to roles matching this date filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<IEnumerable<Grant>> GetAsync(
        string? clientId = null,
        string? resourceServerId = null,
        PaginationFilter? pagination = null, 
        TimestampFilter? dateFilter = null, 
        CancellationToken cancellationToken = default);
}