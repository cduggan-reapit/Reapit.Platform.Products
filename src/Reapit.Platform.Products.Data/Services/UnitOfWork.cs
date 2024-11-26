using Reapit.Platform.Products.Data.Context;
using Reapit.Platform.Products.Data.Repositories.ProductClients;
using Reapit.Platform.Products.Data.Repositories.Products;

namespace Reapit.Platform.Products.Data.Services;

/// <inheritdoc />
public class UnitOfWork : IUnitOfWork
{
    private readonly ProductDbContext _context;
    private IProductRepository? _productRepository;
    private IProductClientRepository? _productClientRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public UnitOfWork(ProductDbContext context)
        => _context = context;

    /// <inheritdoc />
    public IProductRepository Products 
        => _productRepository ??= new ProductRepository(_context);
    
    /// <inheritdoc />
    public IProductClientRepository ProductClients 
        => _productClientRepository ??= new ProductClientRepository(_context);

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
        => await _context.SaveChangesAsync(cancellationToken);
}