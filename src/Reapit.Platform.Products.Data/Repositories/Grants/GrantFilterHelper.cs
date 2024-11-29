using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.Repositories.Grants;

public static class GrantFilterHelper
{
    /// <summary>Applies a cursor offset to a collection of grants.</summary>
    /// <param name="queryable">The collection to limit.</param>
    /// <param name="value">The maximum cursor value of the last result set.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Grant> ApplyCursorFilter(this IQueryable<Grant> queryable, long? value) 
        => value == null 
            ? queryable 
            : queryable.Where(entity => entity.Cursor > value);
    
    /// <summary>Filters a collection of grants by minimum creation date (inclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Grant> ApplyCreatedFromFilter(this IQueryable<Grant> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.DateCreated >= value.Value);
    
    /// <summary>Filters a collection of grants by maximum creation date (exclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Grant> ApplyCreatedToFilter(this IQueryable<Grant> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.DateCreated < value.Value);
    
    /// <summary>Filters a collection of grants by minimum last modified date (inclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Grant> ApplyModifiedFromFilter(this IQueryable<Grant> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.DateModified >= value.Value);
    
    /// <summary>Filters a collection of grants by maximum last modified date (exclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Grant> ApplyModifiedToFilter(this IQueryable<Grant> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.DateModified < value.Value);
    
    /// <summary>Filters a collection of grants matching a given external identifier.</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Grant> ApplyExternalIdFilter(this IQueryable<Grant> queryable, string? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.ExternalId == value);
    
    /// <summary>Filters a collection of grants associated with a client.</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Grant> ApplyClientIdFilter(this IQueryable<Grant> queryable, string? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.ClientId == value);
    
    /// <summary>Filters a collection of grants associated with a resource server.</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Grant> ApplyResourceServerIdFilter(this IQueryable<Grant> queryable, string? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.ResourceServerId == value);
}