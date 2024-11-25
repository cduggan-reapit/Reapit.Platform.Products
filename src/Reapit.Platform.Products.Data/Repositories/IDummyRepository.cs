using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.Repositories;

/// <summary>Data access service for the persistent Dummy store.</summary>
public interface IDummyRepository
{
    /// <summary>Get a collection of Dummies from the database with optional filters applied.</summary>
    /// <param name="name">The name filter to apply.</param>
    /// <param name="createdFrom">The minimum creation date filter to apply.</param>
    /// <param name="createdTo">The maximum creation date filter to apply.</param>
    /// <param name="modifiedFrom">The minimum last modified date filter to apply.</param>
    /// <param name="modifiedTo">The maximum last modified date filter to apply.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public Task<IEnumerable<Dummy>> GetAsync(
        string? name,
        DateTime? createdFrom,
        DateTime? createdTo,
        DateTime? modifiedFrom,
        DateTime? modifiedTo,
        CancellationToken cancellationToken);
    
    /// <summary>Get a single Dummy from the database using it's unique identifier.</summary>
    /// <param name="id">The unique identifier of the Dummy.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The Dummy object if a matching object found; otherwise null.</returns>
    public Task<Dummy?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    
    /// <summary>Add a dummy instance to the change tracker.</summary>
    /// <param name="entity">The dummy instance.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <remarks>Requires a call to <see cref="IUnitOfWork.SaveChangesAsync"/> for changes to be persisted.</remarks>
    public Task CreateAsync(Dummy entity, CancellationToken cancellationToken);
    
    /// <summary>Flag an instance of dummy as modified in the change tracker.</summary>
    /// <param name="entity">The dummy instance.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <remarks>Requires a call to <see cref="IUnitOfWork.SaveChangesAsync"/> for changes to be persisted.</remarks>
    public Task UpdateAsync(Dummy entity, CancellationToken cancellationToken);
    
    /// <summary>Flag an instance of dummy as deleted in the change tracker.</summary>
    /// <param name="entity">The dummy instance.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <remarks>Requires a call to <see cref="IUnitOfWork.SaveChangesAsync"/> for changes to be persisted.</remarks>
    public Task DeleteAsync(Dummy entity, CancellationToken cancellationToken);
}