using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Products.Data.Context;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Data.Repositories.ProductClients;

/// <inheritdoc cref="IProductClientRepository" />
public class ProductClientRepository(ProductDbContext context) 
    : BaseRepository<ProductClient>(context), IProductClientRepository
{
    /// <inheritdoc/>
    public async Task<IEnumerable<ProductClient>> GetProductClientsAsync(
        string? name = null, 
        string? description = null, 
        string? productId = null,
        string? clientId = null, 
        string? grantId = null,
        ClientType? type = null,
        PaginationFilter? pagination = null,
        TimestampFilter? dateFilter = null, 
        CancellationToken cancellationToken = default)
        => await context.ProductClients
            .ApplyCursorFilter(pagination?.Cursor)
            .ApplyNameFilter(name)
            .ApplyDescriptionFilter(description)
            .ApplyProductIdFilter(productId)
            .ApplyClientIdFilter(clientId)
            .ApplyGrantIdFilter(grantId)
            .ApplyTypeFilter(type)
            .ApplyCreatedFromFilter(dateFilter?.CreatedFrom)
            .ApplyCreatedToFilter(dateFilter?.CreatedTo)
            .ApplyModifiedFromFilter(dateFilter?.ModifiedFrom)
            .ApplyModifiedToFilter(dateFilter?.ModifiedTo)
            .OrderBy(entity => entity.Cursor)
            .Take(pagination?.PageSize ?? 25)
            .ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<ProductClient?> GetProductClientByIdAsync(string id, CancellationToken cancellationToken)
        => await context.ProductClients
            .Include(client => client.Product)
            .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
}