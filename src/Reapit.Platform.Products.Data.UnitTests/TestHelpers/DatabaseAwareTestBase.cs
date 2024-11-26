using Reapit.Platform.Products.Data.Context;
using Reapit.Platform.Products.Data.UnitTests.Context;

namespace Reapit.Platform.Products.Data.UnitTests.TestHelpers;

public abstract class DatabaseAwareTestBase
{
    private readonly TestDbContextFactory _contextFactory = new();
    private ProductDbContext? _context;

    public ProductDbContext GetContext(bool ensureCreated = true)
        => _context ??= _contextFactory.CreateContext(ensureCreated);
    
    public async Task<ProductDbContext> GetContextAsync(bool ensureCreated = true, CancellationToken cancellationToken = default)
        => _context ??= await _contextFactory.CreateContextAsync(ensureCreated, cancellationToken);
}