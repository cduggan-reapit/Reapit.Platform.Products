using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Products.Data.Context;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Data.UnitTests.TestHelpers;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.UnitTests.Services;

public class UnitOfWorkTests : DatabaseAwareTestBase
{
    // /*
    //  * Products
    //  */
    //
    // [Fact]
    // public async Task Products_ReturnsRepository_WhenCalledForTheFirstTime()
    // {
    //     await using var dbContext = await GetContextAsync();
    //     var sut = CreateSut(dbContext);
    //     var actual = sut.Products;
    //     actual.Should().NotBeNull();
    // }
    //
    // [Fact]
    // public async Task Products_ReusesRepository_ForSubsequentCalls()
    // {
    //     await using var dbContext = await GetContextAsync();
    //     var sut = CreateSut(dbContext);
    //     var initial = sut.Products;
    //     var subsequent = sut.Products;
    //     subsequent.Should().BeSameAs(initial);
    // }
    //
    // /*
    //  * SaveChangesAsync
    //  */
    //
    // [Fact]
    // public async Task SaveChangesAsync_CommitsChangesToDatabase_WhenCalledAfterChangesMadeInRepository()
    // {
    //     var entity = new ResourceServer("dummy name", null);
    //     
    //     await using var dbContext = await GetContextAsync();
    //     var sut = CreateSut(dbContext);
    //     
    //     // CreateAsync should add one - check that it's state is Added
    //     await sut.Products.CreateAsync(entity, default);
    //     dbContext.ChangeTracker.Entries().Should().AllSatisfy(entry => entry.State .Should().Be(EntityState.Added));
    //     
    //     await sut.SaveChangesAsync(default);
    //
    //     // Once it's saved, it should be committed and thus tracked as Unchanged
    //     dbContext.Products.Should().HaveCount(1);
    //     dbContext.ChangeTracker.Entries().Should().AllSatisfy(entry => entry.State .Should().Be(EntityState.Unchanged));
    // }
    //
    // [Fact]
    // public async Task SaveChangesAsync_DoesNotThrow_WhenNoChangesTracked()
    // {
    //     await using var dbContext = await GetContextAsync();
    //     var sut = CreateSut(dbContext);
    //     
    //     await sut.SaveChangesAsync(default);
    //     dbContext.Products.Should().HaveCount(0);
    // }
    
    /*
     * Private methods
     */

    private static UnitOfWork CreateSut(ProductDbContext context)
        => new(context);
}