using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Products.Data.Context;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.Repositories.Grants;

public class GrantRepository(ProductDbContext context) 
    : BaseRepository<Grant>(context), IGrantRepository
{
    /// <inheritdoc/>
    public override async Task<Grant?> GetByIdAsync(string id, CancellationToken cancellationToken)
        => await context.Grants
            .Include(grant => grant.Client)
            .Include(grant => grant.ResourceServer)
            .Include(grant => grant.Scopes)
            .SingleOrDefaultAsync(grant => grant.Id == id, cancellationToken);

    /// <inheritdoc/>
    public async Task<IEnumerable<Grant>> GetAsync(
        string? clientId = null,
        string? resourceServerId = null,
        PaginationFilter? pagination = null, 
        TimestampFilter? dateFilter = null,
        CancellationToken cancellationToken = default)
        => await context.Grants
            .ApplyCursorFilter(pagination?.Cursor)
            .ApplyClientIdFilter(clientId)
            .ApplyResourceServerIdFilter(resourceServerId)
            .ApplyCreatedFromFilter(dateFilter?.CreatedFrom)
            .ApplyCreatedToFilter(dateFilter?.CreatedTo)
            .ApplyModifiedFromFilter(dateFilter?.ModifiedFrom)
            .ApplyModifiedToFilter(dateFilter?.ModifiedTo)
            .OrderBy(entity => entity.Cursor)
            .Take(pagination?.PageSize ?? 25)
            .ToListAsync(cancellationToken);
}