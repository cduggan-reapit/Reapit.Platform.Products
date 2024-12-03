using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Data.Repositories.Clients;

/// <summary>Helper for client queries.</summary>
public static class ClientFilterHelper
{
    /// <summary>Applies a cursor offset to a collection of clients.</summary>
    /// <param name="queryable">The collection to limit.</param>
    /// <param name="value">The maximum cursor value of the last result set.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Client> ApplyCursorFilter(this IQueryable<Client> queryable, long? value) 
        => value == null 
            ? queryable 
            : queryable.Where(entity => entity.Cursor > value);
    
    /// <summary>Filters a collection of clients by application id.</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Client> ApplyAppIdFilter(this IQueryable<Client> queryable, string? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.AppId == value);
    
    /// <summary>Filters a collection of clients by name.</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Client> ApplyNameFilter(this IQueryable<Client> queryable, string? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.Name == value);
    
    /// <summary>Filters a collection of clients by description.</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Client> ApplyDescriptionFilter(this IQueryable<Client> queryable, string? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.Description != null && entity.Description.Contains(value));
    
    /// <summary>Filters a collection of clients by type.</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Client> ApplyTypeFilter(this IQueryable<Client> queryable, ClientType? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.Type == value);
    
    /// <summary>Filters a collection of clients by minimum creation date (inclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Client> ApplyCreatedFromFilter(this IQueryable<Client> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.DateCreated >= value.Value);
    
    /// <summary>Filters a collection of clients by maximum creation date (exclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Client> ApplyCreatedToFilter(this IQueryable<Client> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.DateCreated < value.Value);
    
    /// <summary>Filters a collection of clients by minimum last modified date (inclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Client> ApplyModifiedFromFilter(this IQueryable<Client> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.DateModified >= value.Value);
    
    /// <summary>Filters a collection of clients by maximum last modified date (exclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Client> ApplyModifiedToFilter(this IQueryable<Client> queryable, DateTime? value)
        => value == null
            ? queryable
            : queryable.Where(entity => entity.DateModified < value.Value);
}