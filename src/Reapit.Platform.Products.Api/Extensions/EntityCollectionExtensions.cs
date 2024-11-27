using Reapit.Platform.Products.Domain.Entities.Interfaces;

namespace Reapit.Platform.Products.Api.Extensions;

/// <summary>Extension methods for collections of entities.</summary>
public static class EntityCollectionExtensions
{
    /// <summary>Gets the maximum cursor value from a collection of <typeparamref name="TEntity"/>.</summary>
    /// <param name="collection">The collection.</param>
    /// <returns>The maximum Cursor value from the collection if it contains any items; otherwise zero.</returns>
    public static long GetMaximumCursor<TEntity>(this IEnumerable<TEntity> collection)
        where TEntity : IHasCursor
    {
        var list = collection.ToList();
        return list.Any() ? list.Max(item => item.Cursor) : 0;
    }
}