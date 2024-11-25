using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.Helpers;

public static class DummyFilterHelper
{
    /// <summary>Filters a collection of <see cref="Dummy"/> objects by name.</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Dummy> ApplyNameFilter(this IQueryable<Dummy> queryable, string? value) 
        => value == null 
            ? queryable 
            : queryable.Where(dummy => dummy.Name == value);

    /// <summary>Filters a collection of <see cref="Dummy"/> objects by minimum creation date (inclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Dummy> ApplyCreatedFromFilter(this IQueryable<Dummy> queryable, DateTime? value) 
        => value == null 
            ? queryable 
            : queryable.Where(dummy => dummy.DateCreated >= value);

    /// <summary>Filters a collection of <see cref="Dummy"/> objects by maximum creation date (exclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Dummy> ApplyCreatedToFilter(this IQueryable<Dummy> queryable, DateTime? value) 
        => value == null 
            ? queryable 
            : queryable.Where(dummy => dummy.DateCreated < value);
    
    /// <summary>Filters a collection of <see cref="Dummy"/> objects by minimum last modified date (inclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Dummy> ApplyModifiedFromFilter(this IQueryable<Dummy> queryable, DateTime? value) 
        => value == null 
            ? queryable 
            : queryable.Where(dummy => dummy.DateModified >= value);

    /// <summary>Filters a collection of <see cref="Dummy"/> objects by maximum last modified date (exclusive).</summary>
    /// <param name="queryable">The collection to filter.</param>
    /// <param name="value">The value to filter by.</param>
    /// <returns>A reference to the queryable after the filter operation.</returns>
    public static IQueryable<Dummy> ApplyModifiedToFilter(this IQueryable<Dummy> queryable, DateTime? value) 
        => value == null 
            ? queryable 
            : queryable.Where(dummy => dummy.DateModified < value);
}