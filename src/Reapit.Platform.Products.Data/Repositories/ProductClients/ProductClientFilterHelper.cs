using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Data.Repositories.ProductClients;

/// <summary>
/// Filter helper for <see cref="ProductClient"/> queries.
/// </summary>
public static class ProductClientFilterHelper
{
    /// <summary>Applies a cursor offset to a collection of <see cref="ProductClient"/>.</summary>
    /// <param name="queryable">The collection to limit.</param>
    /// <param name="value">The maximum cursor value of the last result set.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<ProductClient> ApplyCursorFilter(this IQueryable<ProductClient> queryable, long? value) 
        => value == null 
            ? queryable 
            : queryable.Where(entity => entity.Cursor > value);
    
    /// <summary>Filters a collection of <see cref="ProductClient"/> objects by name.</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<ProductClient> ApplyNameFilter(this IQueryable<ProductClient> queryable, string? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.Name == value);
    
    /// <summary>Filters a collection of <see cref="ProductClient"/> objects by description.</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<ProductClient> ApplyDescriptionFilter(this IQueryable<ProductClient> queryable, string? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.Description != null && entity.Description.Contains(value));
    
    /// <summary>Filters a collection of <see cref="ProductClient"/> objects by parent product identifier.</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<ProductClient> ApplyProductIdFilter(this IQueryable<ProductClient> queryable, string? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.ProductId == value);
    
    /// <summary>Filters a collection of <see cref="ProductClient"/> objects by client type.</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<ProductClient> ApplyTypeFilter(this IQueryable<ProductClient> queryable, ClientType? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.Type == value);
    
    /// <summary>Filters a collection of <see cref="ProductClient" /> objects by minimum creation date (inclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<ProductClient> ApplyCreatedFromFilter(this IQueryable<ProductClient> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.DateCreated >= value.Value);
    
    /// <summary>Filters a collection of <see cref="ProductClient" /> objects by maximum creation date (exclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<ProductClient> ApplyCreatedToFilter(this IQueryable<ProductClient> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.DateCreated < value.Value);
    
    /// <summary>Filters a collection of <see cref="ProductClient" /> objects by minimum last modified date (inclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<ProductClient> ApplyModifiedFromFilter(this IQueryable<ProductClient> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.DateModified >= value.Value);
    
    /// <summary>Filters a collection of <see cref="ProductClient" /> objects by maximum last modified date (exclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<ProductClient> ApplyModifiedToFilter(this IQueryable<ProductClient> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.DateModified < value.Value);
}