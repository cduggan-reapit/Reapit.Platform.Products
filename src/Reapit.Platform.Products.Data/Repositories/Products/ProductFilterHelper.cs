using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.Repositories.Products;

/// <summary>
/// Filter helper for <see cref="Product"/> queries.
/// </summary>
public static class ProductFilterHelper
{
    /// <summary>Applies a cursor offset to a collection of <see cref="Product"/>.</summary>
    /// <param name="queryable">The collection to limit.</param>
    /// <param name="value">The maximum cursor value of the last result set.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Product> ApplyCursorFilter(this IQueryable<Product> queryable, long? value) 
        => value == null 
            ? queryable 
            : queryable.Where(entity => entity.Cursor > value);
    
    /// <summary>Filters a collection of <see cref="Product"/> objects by name.</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Product> ApplyNameFilter(this IQueryable<Product> queryable, string? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.Name == value);
    
    /// <summary>Filters a collection of <see cref="Product"/> objects by description.</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Product> ApplyDescriptionFilter(this IQueryable<Product> queryable, string? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.Description != null && entity.Description.Contains(value));
    
    /// <summary>Filters a collection of <see cref="Product" /> objects by minimum creation date (inclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Product> ApplyCreatedFromFilter(this IQueryable<Product> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.DateCreated >= value.Value);
    
    /// <summary>Filters a collection of <see cref="Product" /> objects by maximum creation date (exclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Product> ApplyCreatedToFilter(this IQueryable<Product> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.DateCreated < value.Value);
    
    /// <summary>Filters a collection of <see cref="Product" /> objects by minimum last modified date (inclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Product> ApplyModifiedFromFilter(this IQueryable<Product> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.DateModified >= value.Value);
    
    /// <summary>Filters a collection of <see cref="Product" /> objects by maximum last modified date (exclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Product> ApplyModifiedToFilter(this IQueryable<Product> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.DateModified < value.Value);
}