using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Products.Data.Context;

namespace Reapit.Platform.Products.Data.UnitTests.Context;

public class TestDbContextFactory : IDisposable, IAsyncDisposable
{
    private readonly SqliteConnection _connection = new("Filename=:memory:");
    
    public DemoDbContext CreateContext(bool ensureCreated = true)
    {
        _connection.Open();
        var context = InstantiateDbContext();
        
        if (!ensureCreated) 
            return context;
        
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        return context;
    }

    public async Task<DemoDbContext> CreateContextAsync(
        bool ensureCreated = true, 
        CancellationToken cancellationToken = default)
    {
        await _connection.OpenAsync(cancellationToken);
        var context = InstantiateDbContext();

        if (!ensureCreated) 
            return context;
        
        await context.Database.EnsureDeletedAsync(cancellationToken);
        await context.Database.EnsureCreatedAsync(cancellationToken);
        return context;
    }
    
    public void Dispose()
        => _connection.Dispose();

    public async ValueTask DisposeAsync()
        => await _connection.DisposeAsync();

    private DemoDbContext InstantiateDbContext()
        => new(new DbContextOptionsBuilder<DemoDbContext>().UseSqlite(_connection).Options);
}