namespace Reapit.Platform.Products.Core.UseCases.ResourceServers.GetResourceServers;

/// <summary>Request to get a collection of resource servers matching the provided filters.</summary>
/// <param name="Cursor">The offset cursor; default 0.</param>
/// <param name="PageSize">The maximum number of results to return; default 25.</param>
/// <param name="Name">Limit results to records matching this name.</param>
/// <param name="Name">Limit results to records matching this audience.</param>
/// <param name="CreatedFrom">Limit results to records created on or after this date (UTC).</param>
/// <param name="CreatedTo">Limit results to records created before this date (UTC).</param>
/// <param name="ModifiedFrom">Limit results to records last modified on or after this date.</param>
/// <param name="ModifiedTo">Limit results to records last modified before this date.</param>
public record GetResourceServersQuery(
    long? Cursor = null,
    int PageSize = 25,
    string? Name = null,
    string? Audience = null,
    DateTime? CreatedFrom = null,
    DateTime? CreatedTo = null,
    DateTime? ModifiedFrom = null,
    DateTime? ModifiedTo = null) 
    : IRequest<IEnumerable<Entities.ResourceServer>>;