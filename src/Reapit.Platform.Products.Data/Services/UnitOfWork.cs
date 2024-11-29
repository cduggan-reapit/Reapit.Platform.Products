using Reapit.Platform.Products.Data.Context;

namespace Reapit.Platform.Products.Data.Services;

/// <inheritdoc />
public class UnitOfWork : IUnitOfWork
{
    private readonly ProductDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public UnitOfWork(ProductDbContext context)
        => _context = context;

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
        => await _context.SaveChangesAsync(cancellationToken);
}