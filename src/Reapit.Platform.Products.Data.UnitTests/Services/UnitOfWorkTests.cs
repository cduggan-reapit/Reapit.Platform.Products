using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Products.Data.Context;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Data.UnitTests.TestHelpers;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.UnitTests.Services;

public class UnitOfWorkTests : DatabaseAwareTestBase
{
    /*
     * Dummies
     */

    // [Fact]
    // public async Task Dummies_ReturnsRepository_WhenCalledForTheFirstTime()
    // {
    //     await using var dbContext = await GetContextAsync();
    //     var sut = CreateSut(dbContext);
    //     var actual = sut.Dummies;
    //     actual.Should().NotBeNull();
    // }
    //
    // [Fact]
    // public async Task Dummies_ReusesRepository_ForSubsequentCalls()
    // {
    //     await using var dbContext = await GetContextAsync();
    //     var sut = CreateSut(dbContext);
    //     var initial = sut.Dummies;
    //     var subsequent = sut.Dummies;
    //     subsequent.Should().BeSameAs(initial);
    // }
    
    /*
     * SaveChangesAsync
     */

    // [Fact]
    // public async Task SaveChangesAsync_CommitsChangesToDatabase_WhenCalledAfterChangesMadeInRepository()
    // {
    //     var dummy = new Dummy("dummy name");
    //     
    //     await using var dbContext = await GetContextAsync();
    //     var sut = CreateSut(dbContext);
    //     
    //     // CreateAsync should add one - check that it's state is Added
    //     await sut.Dummies.CreateAsync(dummy, default);
    //     dbContext.ChangeTracker.Entries().Should().AllSatisfy(entry => entry.State .Should().Be(EntityState.Added));
    //     
    //     await sut.SaveChangesAsync(default);
    //
    //     // Once it's saved, it should be committed and thus tracked as Unchanged
    //     dbContext.Dummies.Should().HaveCount(1);
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
    //     dbContext.Dummies.Should().HaveCount(0);
    // }
    
    /*
     * Private methods
     */

    private static UnitOfWork CreateSut(DemoDbContext context)
        => new(context);
}