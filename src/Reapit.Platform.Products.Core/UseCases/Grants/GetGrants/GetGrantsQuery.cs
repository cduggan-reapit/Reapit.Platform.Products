namespace Reapit.Platform.Products.Core.UseCases.Grants.GetGrants;

/// <summary>Request to get a collection of resource servers matching the provided filters.</summary>
/// <param name="Cursor">The offset cursor; default 0.</param>
/// <param name="PageSize">The maximum number of results to return; default 25.</param>
/// <param name="ClientId">Limit results to records associated with the client with this unique identifier.</param>
/// <param name="ResourceServerId">Limit results to records associated with the resource server with this unique identifier.</param>
/// <param name="CreatedFrom">Limit results to records created on or after this date (UTC).</param>
/// <param name="CreatedTo">Limit results to records created before this date (UTC).</param>
/// <param name="ModifiedFrom">Limit results to records last modified on or after this date.</param>
/// <param name="ModifiedTo">Limit results to records last modified before this date.</param>
public record GetGrantsQuery(
    long? Cursor = null,
    int PageSize = 25,
    string? ClientId = null,
    string? ResourceServerId = null,
    DateTime? CreatedFrom = null,
    DateTime? CreatedTo = null,
    DateTime? ModifiedFrom = null,
    DateTime? ModifiedTo = null) 
    : IRequest<IEnumerable<Entities.Grant>>;