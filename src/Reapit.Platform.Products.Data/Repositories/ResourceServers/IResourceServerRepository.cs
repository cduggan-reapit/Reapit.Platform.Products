using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.Repositories.ResourceServers;

/// <summary>Repository service for the <see cref="ResourceServer"/> type.</summary>
public interface IResourceServerRepository : IBaseRepository<ResourceServer>
{
    /// <summary>Get a collection of resource servers with optional filters applied.</summary>
    /// <param name="externalId">Limit results to records matching this external identifier.</param>
    /// <param name="name">Limit results to records matching this name.</param>
    /// <param name="audience">Limit results to records matching this audience value.</param>
    /// <param name="pagination">Limit results to a page matching this pagination filter.</param>
    /// <param name="dateFilter">Limit results to roles matching this date filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<IEnumerable<ResourceServer>> GetAsync(
        string? externalId = null,
        string? name = null,
        string? audience = null,
        PaginationFilter? pagination = null,
        TimestampFilter? dateFilter = null,
        CancellationToken cancellationToken = default);
}