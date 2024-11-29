using Reapit.Platform.Products.Data.Context;
using Reapit.Platform.Products.Data.Repositories.Apps;
using Reapit.Platform.Products.Data.Repositories.Clients;
using Reapit.Platform.Products.Data.Repositories.ResourceServers;

namespace Reapit.Platform.Products.Data.Services;

/// <inheritdoc />
public class UnitOfWork : IUnitOfWork
{
    private readonly ProductDbContext _context;
    private IAppRepository? _apps;
    private IClientRepository? _clients;
    private IResourceServerRepository? _resourceServers;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public UnitOfWork(ProductDbContext context)
        => _context = context;

    /// <inheritdoc />
    public IAppRepository Apps 
        => _apps ??= new AppRepository(_context);

    /// <inheritdoc />
    public IClientRepository Clients 
        => _clients ??= new ClientRepository(_context);

    /// <inheritdoc />
    public IResourceServerRepository ResourceServers 
        => _resourceServers ??= new ResourceServerRepository(_context);

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
        => await _context.SaveChangesAsync(cancellationToken);
}