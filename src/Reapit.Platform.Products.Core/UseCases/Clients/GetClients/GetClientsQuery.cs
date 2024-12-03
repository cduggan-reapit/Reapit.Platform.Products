using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Core.UseCases.Clients.GetClients;

/// <summary>Request for a page of clients with optional filters.</summary>
/// <param name="Cursor">The offset cursor; default 0.</param>
/// <param name="PageSize">The maximum number of results to return; default 25.</param>
/// <param name="AppId">Limit results to records associated with the application with this unique identifier.</param>
/// <param name="Type">The type of claim.</param>
/// <param name="Name">Limit results to records matching this name.</param>
/// <param name="Description">Limit results to records matching this description.</param>
/// <param name="CreatedFrom">Limit results to records created on or after this date (UTC).</param>
/// <param name="CreatedTo">Limit results to records created before this date (UTC).</param>
/// <param name="ModifiedFrom">Limit results to records last modified on or after this date.</param>
/// <param name="ModifiedTo">Limit results to records last modified before this date.</param>
public record GetClientsQuery(
    long? Cursor = null,
    int PageSize = 25,
    string? AppId = null, 
    string? Type = null, 
    string? Name = null, 
    string? Description = null,
    DateTime? CreatedFrom = null,
    DateTime? CreatedTo = null,
    DateTime? ModifiedFrom = null,
    DateTime? ModifiedTo = null)
    : IRequest<IEnumerable<Entities.Client>>;