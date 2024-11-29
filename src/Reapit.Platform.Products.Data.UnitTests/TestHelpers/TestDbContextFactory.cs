using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Products.Data.Context;

namespace Reapit.Platform.Products.Data.UnitTests.TestHelpers;

public class TestDbContextFactory : IDisposable, IAsyncDisposable
{
    private readonly SqliteConnection _connection = new("Filename=:memory:");
    
    public async Task<ProductDbContext> CreateContextAsync(
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

    private ProductDbContext InstantiateDbContext()
        => new(new DbContextOptionsBuilder<ProductDbContext>()
            .UseSqlite(_connection)
            .EnableSensitiveDataLogging()
            .Options);
}