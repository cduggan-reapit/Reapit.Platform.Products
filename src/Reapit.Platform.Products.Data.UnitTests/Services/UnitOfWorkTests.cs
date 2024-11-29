using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Products.Data.Context;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.UnitTests.Services;

public class UnitOfWorkTests : DatabaseAwareTestBase
{
    /*
     * Apps
     */
    
    [Fact]
    public async Task Apps_ReturnsRepository_WhenCalledForTheFirstTime()
    {
        await using var dbContext = await GetContextAsync();
        var sut = CreateSut(dbContext);
        var actual = sut.Apps;
        actual.Should().NotBeNull();
    }
    
    [Fact]
    public async Task Apps_ReusesRepository_ForSubsequentCalls()
    {
        await using var dbContext = await GetContextAsync();
        var sut = CreateSut(dbContext);
        var initial = sut.Apps;
        var subsequent = sut.Apps;
        subsequent.Should().BeSameAs(initial);
    }
    
    /*
     * Clients
     */
    
    [Fact]
    public async Task Clients_ReturnsRepository_WhenCalledForTheFirstTime()
    {
        await using var dbContext = await GetContextAsync();
        var sut = CreateSut(dbContext);
        var actual = sut.Clients;
        actual.Should().NotBeNull();
    }
    
    [Fact]
    public async Task Clients_ReusesRepository_ForSubsequentCalls()
    {
        await using var dbContext = await GetContextAsync();
        var sut = CreateSut(dbContext);
        var initial = sut.Clients;
        var subsequent = sut.Clients;
        subsequent.Should().BeSameAs(initial);
    }
    
    /*
     * ResourceServers
     */
    
    [Fact]
    public async Task ResourceServers_ReturnsRepository_WhenCalledForTheFirstTime()
    {
        await using var dbContext = await GetContextAsync();
        var sut = CreateSut(dbContext);
        var actual = sut.ResourceServers;
        actual.Should().NotBeNull();
    }
    
    [Fact]
    public async Task ResourceServers_ReusesRepository_ForSubsequentCalls()
    {
        await using var dbContext = await GetContextAsync();
        var sut = CreateSut(dbContext);
        var initial = sut.ResourceServers;
        var subsequent = sut.ResourceServers;
        subsequent.Should().BeSameAs(initial);
    }
    
    /*
     * SaveChangesAsync
     */
    
    [Fact]
    public async Task SaveChangesAsync_CommitsChangesToDatabase_WhenCalledAfterChangesMadeInRepository()
    {
        var entity = new App("dummy name", null, false);
        
        await using var dbContext = await GetContextAsync();
        var sut = CreateSut(dbContext);
        
        // CreateAsync should add one - check that it's state is Added
        await sut.Apps.CreateAsync(entity, default);
        dbContext.ChangeTracker.Entries().Should().AllSatisfy(entry => entry.State .Should().Be(EntityState.Added));
        
        await sut.SaveChangesAsync(default);
    
        // Once it's saved, it should be committed and thus tracked as Unchanged
        dbContext.Apps.Should().HaveCount(1);
        dbContext.ChangeTracker.Entries().Should().AllSatisfy(entry => entry.State .Should().Be(EntityState.Unchanged));
    }
    
    [Fact]
    public async Task SaveChangesAsync_DoesNotThrow_WhenNoChangesTracked()
    {
        await using var dbContext = await GetContextAsync();
        var sut = CreateSut(dbContext);
        
        await sut.SaveChangesAsync(default);
        dbContext.Apps.Should().HaveCount(0);
    }
    
    /*
     * Private methods
     */

    private static UnitOfWork CreateSut(ProductDbContext context)
        => new(context);
}