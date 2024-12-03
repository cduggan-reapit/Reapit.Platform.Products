using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Data.Repositories.Clients;

/// <summary>Repository service for the <see cref="Client"/> type.</summary>
public interface IClientRepository : IBaseRepository<Client>
{
    /// <summary>Get a collection of clients with optional filters applied.</summary>
    /// <param name="appId">Limit results to records associated with the app with this unique identifier.</param>
    /// <param name="type">Limit results to records of this type.</param>
    /// <param name="name">Limit results to records matching this name.</param>
    /// <param name="description">Limit results to records matching this description value.</param>
    /// <param name="pagination">Limit results to a page matching this pagination filter.</param>
    /// <param name="dateFilter">Limit results to roles matching this date filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<IEnumerable<Client>> GetAsync(
        string? appId = null, 
        ClientType? type = null, 
        string? name = null, 
        string? description = null,
        PaginationFilter? pagination = null, 
        TimestampFilter? dateFilter = null, 
        CancellationToken cancellationToken = default);
}