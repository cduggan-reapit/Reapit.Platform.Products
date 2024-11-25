using Reapit.Platform.Products.Data.Context;

namespace Reapit.Platform.Products.Data.Services;

public class UnitOfWork : IUnitOfWork
{
    private readonly DemoDbContext _context;


    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public UnitOfWork(DemoDbContext context)
        => _context = context;

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
        => await _context.SaveChangesAsync(cancellationToken);
}