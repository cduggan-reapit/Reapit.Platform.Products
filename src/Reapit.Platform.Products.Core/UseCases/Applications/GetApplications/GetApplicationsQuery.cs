namespace Reapit.Platform.Products.Core.UseCases.Applications.GetApplications;

/// <summary>Request for a page of applications.</summary>
/// <param name="Cursor">The offset cursor; default 0.</param>
/// <param name="PageSize">The maximum number of results to return; default 25.</param>
/// <param name="Name">Limit results to records matching this name.</param>
/// <param name="Description">Limit results to records matching this description.</param>
/// <param name="IsFirstParty">Limit results to records matching this first party flag.</param>
/// <param name="CreatedFrom">Limit results to records created on or after this date (UTC).</param>
/// <param name="CreatedTo">Limit results to records created before this date (UTC).</param>
/// <param name="ModifiedFrom">Limit results to records last modified on or after this date.</param>
/// <param name="ModifiedTo">Limit results to records last modified before this date.</param>
public record GetApplicationsQuery(
    long? Cursor = null,
    int PageSize = 25,
    string? Name = null,
    string? Description = null,
    bool? IsFirstParty = false,
    DateTime? CreatedFrom = null,
    DateTime? CreatedTo = null,
    DateTime? ModifiedFrom = null,
    DateTime? ModifiedTo = null)
    : IRequest<IEnumerable<Entities.App>>;