using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Products.Data.Context;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.Repositories.Products;

/// <inheritdoc cref="IProductRepository" />
public class ProductRepository(ProductDbContext context) : BaseRepository<Product>(context), IProductRepository
{
    /// <inheritdoc/>
    public async Task<IEnumerable<Product>> GetProductsAsync(
        string? name = null, 
        string? description = null, 
        PaginationFilter? pagination = null,
        TimestampFilter? dateFilter = null, 
        CancellationToken cancellationToken = default)
        => await context.Products
            .ApplyCursorFilter(pagination?.Cursor)
            .ApplyNameFilter(name)
            .ApplyDescriptionFilter(description)
            .ApplyCreatedFromFilter(dateFilter?.CreatedFrom)
            .ApplyCreatedToFilter(dateFilter?.CreatedTo)
            .ApplyModifiedFromFilter(dateFilter?.ModifiedFrom)
            .ApplyModifiedToFilter(dateFilter?.ModifiedTo)
            .OrderBy(entity => entity.Cursor)
            .Take(pagination?.PageSize ?? 25)
            .ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<Product?> GetProductByIdAsync(string id, CancellationToken cancellationToken)
        => await context.Products.SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
}