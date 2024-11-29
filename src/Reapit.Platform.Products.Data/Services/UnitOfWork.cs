using Reapit.Platform.Products.Data.Context;
using Reapit.Platform.Products.Data.Repositories.Products;

namespace Reapit.Platform.Products.Data.Services;

/// <inheritdoc />
public class UnitOfWork : IUnitOfWork
{
    private readonly ProductDbContext _context;
    private IProductRepository? _productRepository;

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
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
        => await _context.SaveChangesAsync(cancellationToken);
}