using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Products.Data.Context;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.UnitTests.TestHelpers;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.UnitTests.Repositories;

public class DummyRepositoryTests : DatabaseAwareTestBase
{
    /*
     * GetAsync
     */

    [Fact]
    public async Task GetAsync_ReturnsEmptyCollection_WhenDatabaseEmpty()
    {
        await using var context = await GetContextAsync();
        var sut = CreateSut(context);
        var actual = await sut.GetAsync(null, null, null, null, null, default);
        actual.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetAsync_ReturnsUnfilteredCollection_WhenNoFilterApplied()
    {
        var seedData = GetDummyCollection(100);
        
        await using var context = await GetContextAsync();
        await context.Dummies.AddRangeAsync(seedData);
        await context.SaveChangesAsync();
        
        var sut = CreateSut(context);
        var actual = await sut.GetAsync(null, null, null, null, null, default);
        actual.Should().BeEquivalentTo(seedData);
    }
    
    [Fact]
    public async Task GetAsync_ReturnsFilteredCollection_WhenNameFilterApplied()
    {
        const string name = "test-dummy-006";
        var seedData = GetDummyCollection(10);
        
        await using var context = await GetContextAsync();
        await context.Dummies.AddRangeAsync(seedData);
        await context.SaveChangesAsync();
        
        var sut = CreateSut(context);
        var actual = await sut.GetAsync(name, null, null, null, null, default);
        actual.Should().HaveCount(1)
            .And.AllSatisfy(item => item.Name.Should().Be(name));
    }
    
    [Fact]
    public async Task GetAsync_ReturnsFilteredCollection_WhenCreatedFromFilterApplied()
    {
        var filterDate = new DateTime(2020, 1, 5);
        var seedData = GetDummyCollection(10);
        
        await using var context = await GetContextAsync();
        await context.Dummies.AddRangeAsync(seedData);
        await context.SaveChangesAsync();
        
        var sut = CreateSut(context);
        var actual = await sut.GetAsync(null, filterDate, null, null, null, default);
        
        // CreatedFrom is inclusive, we expect the 5th, 6th, 7th, 8th, 9th, and 10th to be returned
        actual.Should().HaveCount(6)
            .And.AllSatisfy(item => item.DateCreated.Should().BeOnOrAfter(filterDate));
    }
    
    [Fact]
    public async Task GetAsync_ReturnsFilteredCollection_WhenCreatedToFilterApplied()
    {
        var filterDate = new DateTime(2020, 1, 5);
        var seedData = GetDummyCollection(10);
        
        await using var context = await GetContextAsync();
        await context.Dummies.AddRangeAsync(seedData);
        await context.SaveChangesAsync();
        
        var sut = CreateSut(context);
        var actual = await sut.GetAsync(null, null, filterDate, null, null, default);
        
        // CreatedTo is exclusive, we expect the 1st, 2nd, 3rd, and 4th to be returned
        actual.Should().HaveCount(4)
            .And.AllSatisfy(item => item.DateCreated.Should().BeBefore(filterDate));
    }
    
    [Fact]
    public async Task GetAsync_ReturnsFilteredCollection_WhenModifiedFromFilterApplied()
    {
        var filterDate = new DateTime(2021, 1, 5);
        var seedData = GetDummyCollection(10);
        
        await using var context = await GetContextAsync();
        await context.Dummies.AddRangeAsync(seedData);
        await context.SaveChangesAsync();
        
        var sut = CreateSut(context);
        var actual = await sut.GetAsync(null, null, null, filterDate, null, default);
        
        // ModifiedFrom is inclusive, we expect the 5th, 6th, 7th, 8th, 9th, and 10th to be returned
        actual.Should().HaveCount(6)
            .And.AllSatisfy(item => item.DateModified.Should().BeOnOrAfter(filterDate));
    }
    
    [Fact]
    public async Task GetAsync_ReturnsFilteredCollection_WhenModifiedToFilterApplied()
    {
        var filterDate = new DateTime(2021, 1, 5);
        var seedData = GetDummyCollection(10);
        
        await using var context = await GetContextAsync();
        await context.Dummies.AddRangeAsync(seedData);
        await context.SaveChangesAsync();
        
        var sut = CreateSut(context);
        var actual = await sut.GetAsync(null, null, null, null, filterDate, default);
        
        // ModifiedTo is exclusive, we expect the 1st, 2nd, 3rd, and 4th to be returned
        actual.Should().HaveCount(4)
            .And.AllSatisfy(item => item.DateModified.Should().BeBefore(filterDate));
    }

    
    /*
     * GetByIdAsync
     */

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenEntityNotFound()
    {
        var id = Guid.NewGuid();
        await using var context = await GetContextAsync();
        var sut = CreateSut(context);
        var actual = await sut.GetByIdAsync(id, default);
        actual.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEntity_WhenEntityFound()
    {
        var id = new Guid("00000000-0000-0000-0000-000000000003");
        var seedData = Enumerable.Range(1, 5)
            .Select(i => new Dummy
            {
                Id = new Guid($"00000000-0000-0000-0000-{i:D12}"),
                Name = $"Demo Dummy {i:D3}",
                DateCreated = DateTime.UnixEpoch.AddDays(i),
                DateModified = DateTime.UnixEpoch.AddMonths(i)
            })
            .ToList();
        
        await using var context = await GetContextAsync();
        await context.Dummies.AddRangeAsync(seedData);
        await context.SaveChangesAsync();
        
        var sut = CreateSut(context);
        var actual = await sut.GetByIdAsync(id, default);
        actual.Should().NotBeNull();
        actual?.Id.Should().Be(id);
    }
    
    /*
     * CreateAsync
     */
    
    [Fact]
    public async Task CreateAsync_AddsEntityToChangeTracking()
    {
        var dummy = new Dummy("test name");
        
        await using var context = await GetContextAsync();
        var sut = CreateSut(context);
        await sut.CreateAsync(dummy, default);

        context.ChangeTracker.Entries().Should().HaveCount(1)
            .And.AllSatisfy(entry => entry.State.Should().Be(EntityState.Added));
    }
    
    /*
     * UpdateAsync
     */
    
    [Fact]
    public async Task UpdateAsync_MarksEntityAsUpdated_WithChangeTracker()
    {
        var dummy = new Dummy("test name");
        
        // Add the dummy to the context
        await using var context = await GetContextAsync();
        await context.Dummies.AddAsync(dummy);
        await context.SaveChangesAsync();
        
        // Mark it as updated
        var sut = CreateSut(context);
        await sut.UpdateAsync(dummy, default);

        context.ChangeTracker.Entries().Should().HaveCount(1)
            .And.AllSatisfy(entry => entry.State.Should().Be(EntityState.Modified));
    }
    
    /*
     * DeleteAsync
     */
    
    [Fact]
    public async Task DeleteAsync_MarksEntityAsDeleted_WithChangeTracker()
    {
        var dummy = new Dummy("test name");
        
        // Add the dummy to the context
        await using var context = await GetContextAsync();
        await context.Dummies.AddAsync(dummy);
        await context.SaveChangesAsync();
        
        // Mark it as updated
        var sut = CreateSut(context);
        await sut.DeleteAsync(dummy, default);

        context.ChangeTracker.Entries().Should().HaveCount(1)
            .And.AllSatisfy(entry => entry.State.Should().Be(EntityState.Deleted));
    }
    
    /*
     * Private methods
     */

    private static DummyRepository CreateSut(DemoDbContext dbContext)
        => new(dbContext);
    
    private static readonly DateTime BaseDateTime = DateTime.Parse("2020-01-01");
    
    private static IQueryable<Dummy> GetDummyCollection(int count)
        => Enumerable.Range(1, count)
            .Select(i => new Dummy
            {
                Id = i.ToGuid(),
                Name = $"test-dummy-{i:D3}",
                DateCreated = BaseDateTime.AddDays(i - 1), // Starts from 2020-01-01
                DateModified = BaseDateTime.AddYears(1).AddDays(i - 1) // Starts from 2021-01-01
            })
            .AsQueryable();
}