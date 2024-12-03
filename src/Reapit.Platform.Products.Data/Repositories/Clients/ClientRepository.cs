using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Products.Data.Context;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Data.Repositories.Clients;

/// <inheritdoc cref="IClientRepository" />
public class ClientRepository(ProductDbContext context) : BaseRepository<Client>(context), IClientRepository
{
    /// <inheritdoc />
    public override async Task<Client?> GetByIdAsync(string id, CancellationToken cancellationToken)
        => await context.Clients
            .Include(c => c.App)
            .Include(c => c.Grants)
            .SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<Client>> GetAsync(
        string? appId = null,
        ClientType? type = null,
        string? name = null,
        string? description = null,
        PaginationFilter? pagination = null,
        TimestampFilter? dateFilter = null,
        CancellationToken cancellationToken = default)
        => await context.Clients
            .ApplyCursorFilter(pagination?.Cursor)
            .ApplyAppIdFilter(appId)
            .ApplyTypeFilter(type)
            .ApplyNameFilter(name)
            .ApplyDescriptionFilter(description)
            .ApplyCreatedFromFilter(dateFilter?.CreatedFrom)
            .ApplyCreatedToFilter(dateFilter?.CreatedTo)
            .ApplyModifiedFromFilter(dateFilter?.ModifiedFrom)
            .ApplyModifiedToFilter(dateFilter?.ModifiedTo)
            .OrderBy(entity => entity.Cursor)
            .Take(pagination?.PageSize ?? 25)
            .ToListAsync(cancellationToken);
}