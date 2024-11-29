// using Reapit.Platform.Products.Domain.Entities;
//
// namespace Reapit.Platform.Products.Data.Repositories.Products;
//
// /// <summary>
// /// Filter helper for <see cref="ResourceServer"/> queries.
// /// </summary>
// public static class ProductFilterHelper
// {
//     /// <summary>Applies a cursor offset to a collection of <see cref="ResourceServer"/>.</summary>
//     /// <param name="queryable">The collection to limit.</param>
//     /// <param name="value">The maximum cursor value of the last result set.</param>
//     /// <returns>A reference to the queryable after the filter operation.</returns>
//     public static IQueryable<ResourceServer> ApplyCursorFilter(this IQueryable<ResourceServer> queryable, long? value) 
//         => value == null 
//             ? queryable 
//             : queryable.Where(entity => entity.Cursor > value);
//     
//     /// <summary>Filters a collection of <see cref="ResourceServer"/> objects by name.</summary>
//     /// <param name="queryable">The collection to filter.</param>
//     /// <param name="value">The value to filter by.</param>
//     /// <returns>A reference to the queryable after the filter operation.</returns>
//     public static IQueryable<ResourceServer> ApplyNameFilter(this IQueryable<ResourceServer> queryable, string? value)
//         => value == null
//             ? queryable
//             : queryable.Where(entity => entity.Name == value);
//     
//     /// <summary>Filters a collection of <see cref="ResourceServer"/> objects by description.</summary>
//     /// <param name="queryable">The collection to filter.</param>
//     /// <param name="value">The value to filter by.</param>
//     /// <returns>A reference to the queryable after the filter operation.</returns>
//     public static IQueryable<ResourceServer> ApplyDescriptionFilter(this IQueryable<ResourceServer> queryable, string? value)
//         => value == null
//             ? queryable
//             : queryable.Where(entity => entity.Description != null && entity.Description.Contains(value));
//     
//     /// <summary>Filters a collection of <see cref="ResourceServer" /> objects by minimum creation date (inclusive).</summary>
//     /// <param name="queryable">The collection to filter.</param>
//     /// <param name="value">The value to filter by.</param>
//     /// <returns>A reference to the queryable after the filter operation.</returns>
//     public static IQueryable<ResourceServer> ApplyCreatedFromFilter(this IQueryable<ResourceServer> queryable, DateTime? value)
//         => value == null
//             ? queryable
//             : queryable.Where(entity => entity.DateCreated >= value.Value);
//     
//     /// <summary>Filters a collection of <see cref="ResourceServer" /> objects by maximum creation date (exclusive).</summary>
//     /// <param name="queryable">The collection to filter.</param>
//     /// <param name="value">The value to filter by.</param>
//     /// <returns>A reference to the queryable after the filter operation.</returns>
//     public static IQueryable<ResourceServer> ApplyCreatedToFilter(this IQueryable<ResourceServer> queryable, DateTime? value)
//         => value == null
//             ? queryable
//             : queryable.Where(entity => entity.DateCreated < value.Value);
//     
//     /// <summary>Filters a collection of <see cref="ResourceServer" /> objects by minimum last modified date (inclusive).</summary>
//     /// <param name="queryable">The collection to filter.</param>
//     /// <param name="value">The value to filter by.</param>
//     /// <returns>A reference to the queryable after the filter operation.</returns>
//     public static IQueryable<ResourceServer> ApplyModifiedFromFilter(this IQueryable<ResourceServer> queryable, DateTime? value)
//         => value == null
//             ? queryable
//             : queryable.Where(entity => entity.DateModified >= value.Value);
//     
//     /// <summary>Filters a collection of <see cref="ResourceServer" /> objects by maximum last modified date (exclusive).</summary>
//     /// <param name="queryable">The collection to filter.</param>
//     /// <param name="value">The value to filter by.</param>
//     /// <returns>A reference to the queryable after the filter operation.</returns>
//     public static IQueryable<ResourceServer> ApplyModifiedToFilter(this IQueryable<ResourceServer> queryable, DateTime? value)
//         => value == null
//             ? queryable
//             : queryable.Where(entity => entity.DateModified < value.Value);
// }