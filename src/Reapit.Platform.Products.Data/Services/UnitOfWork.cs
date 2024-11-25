using Reapit.Platform.Products.Data.Context;
using Reapit.Platform.Products.Data.Repositories;

namespace Reapit.Platform.Products.Data.Services;

public class UnitOfWork : IUnitOfWork
{
    private readonly DemoDbContext _context;

    private IDummyRepository? _dummyRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public UnitOfWork(DemoDbContext context)
        => _context = context;
    
    /// <inheritdoc />
    public IDummyRepository Dummies 
        => _dummyRepository ??= new DummyRepository(_context);

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
        => await _context.SaveChangesAsync(cancellationToken);
}