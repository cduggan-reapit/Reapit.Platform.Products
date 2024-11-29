using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.Repositories.Apps;

/// <summary>Repository service for the <see cref="App"/> type.</summary>
public interface IAppRepository : IBaseRepository<App>
{
    /// <summary>Get a collection of apps with optional filters applied.</summary>
    /// <param name="name">Limit results to records matching this name.</param>
    /// <param name="description">Limit results to records matching this description.</param>
    /// <param name="skipConsent">Limit results to records matching this 'skip consent' value.</param>
    /// <param name="pagination">Limit results to a page matching this pagination filter.</param>
    /// <param name="dateFilter">Limit results to roles matching this date filter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<IEnumerable<App>> GetAsync(
        string? name = null,
        string? description = null,
        bool? skipConsent = null,
        PaginationFilter? pagination = null,
        TimestampFilter? dateFilter = null,
        CancellationToken cancellationToken = default);
}