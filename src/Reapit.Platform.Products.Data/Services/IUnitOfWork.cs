using Reapit.Platform.Products.Data.Repositories.Apps;
using Reapit.Platform.Products.Data.Repositories.Clients;
using Reapit.Platform.Products.Data.Repositories.ResourceServers;

namespace Reapit.Platform.Products.Data.Services;

/// <summary>Service managing database transactions.</summary>
public interface IUnitOfWork
{
    /// <inheritdoc cref="IAppRepository" />
    public IAppRepository Apps { get; }
    
    /// <inheritdoc cref="IClientRepository" />
    public IClientRepository Clients { get; }
    
    /// <inheritdoc cref="IResourceServerRepository" />
    public IResourceServerRepository ResourceServers { get; }
    
    /// <summary>Saves all changes made in this context to the database.</summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task SaveChangesAsync(CancellationToken cancellationToken);
}