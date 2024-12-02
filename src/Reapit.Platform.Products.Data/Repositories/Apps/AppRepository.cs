using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Products.Data.Context;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.Repositories.Apps;

/// <inheritdoc cref="IAppRepository" />
public class AppRepository(ProductDbContext context) : BaseRepository<App>(context), IAppRepository
{
    /// <inheritdoc />
    public override async Task<App?> GetByIdAsync(string id, CancellationToken cancellationToken)
        => await context.Apps
            .Include(app => app.Clients)
            .SingleOrDefaultAsync(app => app.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<App>> GetAsync(
        string? name = null,
        string? description = null,
        bool? isFirstParty = null,
        PaginationFilter? pagination = null,
        TimestampFilter? dateFilter = null,
        CancellationToken cancellationToken = default)
        => await context.Apps
            .ApplyCursorFilter(pagination?.Cursor)
            .ApplyNameFilter(name)
            .ApplyDescriptionFilter(description)
            .ApplyIsFirstPartyFilter(isFirstParty)
            .ApplyCreatedFromFilter(dateFilter?.CreatedFrom)
            .ApplyCreatedToFilter(dateFilter?.CreatedTo)
            .ApplyModifiedFromFilter(dateFilter?.ModifiedFrom)
            .ApplyModifiedToFilter(dateFilter?.ModifiedTo)
            .OrderBy(entity => entity.Cursor)
            .Take(pagination?.PageSize ?? 25)
            .ToListAsync(cancellationToken);
}