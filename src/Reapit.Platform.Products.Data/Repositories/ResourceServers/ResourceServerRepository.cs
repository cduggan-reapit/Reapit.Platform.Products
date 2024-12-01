using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Products.Data.Context;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.Repositories.ResourceServers;

/// <inheritdoc cref="IResourceServerRepository" />
public class ResourceServerRepository(ProductDbContext context) 
    : BaseRepository<ResourceServer>(context), IResourceServerRepository
{
    /// <inheritdoc />
    public override async Task<ResourceServer?> GetByIdAsync(string id, CancellationToken cancellationToken)
        => await context.ResourceServers
            .Include(entity => entity.Scopes)
            .SingleOrDefaultAsync(entity => entity.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<ResourceServer>> GetAsync(
        string? name = null,
        string? audience = null,
        PaginationFilter? pagination = null, 
        TimestampFilter? dateFilter = null,
        CancellationToken cancellationToken = default)
        => await context.ResourceServers
            .ApplyCursorFilter(pagination?.Cursor)
            .ApplyNameFilter(name)
            .ApplyAudienceFilter(audience)
            .ApplyCreatedFromFilter(dateFilter?.CreatedFrom)
            .ApplyCreatedToFilter(dateFilter?.CreatedTo)
            .ApplyModifiedFromFilter(dateFilter?.ModifiedFrom)
            .ApplyModifiedToFilter(dateFilter?.ModifiedTo)
            .OrderBy(entity => entity.Cursor)
            .Take(pagination?.PageSize ?? 25)
            .ToListAsync(cancellationToken);

}