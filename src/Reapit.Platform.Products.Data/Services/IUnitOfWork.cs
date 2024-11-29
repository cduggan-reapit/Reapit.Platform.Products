using Reapit.Platform.Products.Data.Repositories.Products;

namespace Reapit.Platform.Products.Data.Services;

/// <summary>Service managing database transactions.</summary>
public interface IUnitOfWork
{
    /// <summary>The product repository.</summary>
    public IProductRepository Products { get; }
    
    /// <summary>Saves all changes made in this context to the database.</summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task SaveChangesAsync(CancellationToken cancellationToken);
}