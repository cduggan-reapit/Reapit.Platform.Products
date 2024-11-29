using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Products.Domain.Entities.Abstract;

namespace Reapit.Platform.Products.Data.Repositories;

/// <summary>Base repository implementation.</summary>
/// <param name="context">The database context.</param>
/// <typeparam name="T">The type of entity managed by the repository.</typeparam>
public abstract class BaseRepository<T>(DbContext context) : IBaseRepository<T>
    where T : EntityBase
{
    /// <inheritdoc />
    public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken)
    {
        await context.Set<T>().AddAsync(entity, cancellationToken);
        return entity;
    } 

    /// <inheritdoc />
    public Task<T> UpdateAsync(T entity, CancellationToken cancellationToken)
    {
        context.Set<T>().Update(entity);
        return Task.FromResult(entity);
    } 

    /// <inheritdoc />
    public Task<T> DeleteAsync(T entity, CancellationToken cancellationToken)
    {
        context.Set<T>().Remove(entity);
        return Task.FromResult(entity);
    }

    /// <inheritdoc/>
    public abstract Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken);
}