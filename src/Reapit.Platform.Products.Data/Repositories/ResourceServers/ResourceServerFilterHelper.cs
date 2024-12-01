using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.Repositories.ResourceServers;

/// <summary>Helper for resource server queries.</summary>
public static class ResourceServerFilterHelper
{
    /// <summary>Applies a cursor offset to a collection of resource servers.</summary>
    /// <param name="queryable">The collection to limit.</param>
    /// <param name="value">The maximum cursor value of the last result set.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<ResourceServer> ApplyCursorFilter(this IQueryable<ResourceServer> queryable, long? value) 
        => value == null 
            ? queryable 
            : queryable.Where(entity => entity.Cursor > value);
    
    /// <summary>Filters a collection of resource servers by name.</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<ResourceServer> ApplyNameFilter(this IQueryable<ResourceServer> queryable, string? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.Name == value);
    
    /// <summary>Filters a collection of resource servers by audience.</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<ResourceServer> ApplyAudienceFilter(this IQueryable<ResourceServer> queryable, string? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.Audience.Contains(value));
    
    /// <summary>Filters a collection of resource servers by minimum creation date (inclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<ResourceServer> ApplyCreatedFromFilter(this IQueryable<ResourceServer> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.DateCreated >= value.Value);
    
    /// <summary>Filters a collection of resource servers by maximum creation date (exclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<ResourceServer> ApplyCreatedToFilter(this IQueryable<ResourceServer> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.DateCreated < value.Value);
    
    /// <summary>Filters a collection of resource servers by minimum last modified date (inclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<ResourceServer> ApplyModifiedFromFilter(this IQueryable<ResourceServer> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.DateModified >= value.Value);
    
    /// <summary>Filters a collection of resource servers by maximum last modified date (exclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<ResourceServer> ApplyModifiedToFilter(this IQueryable<ResourceServer> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.DateModified < value.Value);
}