using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Data.Context;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Repositories.Apps;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.UnitTests.Repositories.Apps;

public class AppRepositoryTests : DatabaseAwareTestBase
{
    /*
     * GetAsync
     */
    
    [Fact]
    public async Task GetAsync_ReturnsUnfilteredPage_WhenNoParametersProvided()
    {
        // Get the default page size with no offset
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var sut = CreateSut(context);
        var actual = await sut.GetAsync();
        actual.Should().HaveCount(25);
    }

    [Fact]
    public async Task GetAsync_ReturnsUnfilteredPage_WhenPaginationDetailsProvided()
    {
        // Get the default page size with no offset
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        // Skip 5 items, take 5 items
        var expected = context.Apps.OrderBy(r => r.Cursor).Skip(5).Take(5);
        var cursor = await expected.MinAsync(item => item.Cursor) - 1;
        var ids = await expected.Select(item => item.Id).ToListAsync();
        
        var sut = CreateSut(context);
        var actual = await sut.GetAsync(pagination: new PaginationFilter(cursor, 5));
        actual.Should().HaveCount(5)
            .And.AllSatisfy(item => ids.Should().Contain(item.Id));
    }

    [Fact]
    public async Task GetAsync_ReturnsFilteredPage_WhenSkipConsentProvided()
    {
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var sut = CreateSut(context);
        var actual = await sut.GetAsync(skipConsent: true);
        actual.Should().HaveCount(5)
            .And.AllSatisfy(item => item.IsFirstParty.Should().BeTrue());
    }

    [Fact]
    public async Task GetAsync_ReturnsFilteredPage_WhenNameProvided()
    {
        const string name = "App 042";
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var sut = CreateSut(context);
        var actual = await sut.GetAsync(name: name);
        actual.Should().HaveCount(1)
            .And.AllSatisfy(item => item.Name.Should().Be(name));
    }

    [Fact]
    public async Task GetAsync_ReturnsFilteredPage_WhenDescriptionProvided()
    {
        const string description = "App 03";
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var sut = CreateSut(context);
        var actual = await sut.GetAsync(description: description);
        actual.Should().HaveCount(10)
            .And.AllSatisfy(item => item.Description.Should().Contain(description));
    }

    [Fact]
    public async Task GetAsync_ReturnsFilteredPage_WhenCreatedFromProvided()
    {
        // There are 50 items (with created dates from BaseDate +0 to BaseDate +49 days).
        // The createdFrom date is INCLUSIVE, so if we try to get +40 to +49 there should be 10 results:
        // [40, 41, 42, 43, 44, 45, 46, 47, 48, 49]
        var createdFrom = BaseDateTime.AddDays(40);
        var dateFilter = new TimestampFilter(CreatedFrom: createdFrom.UtcDateTime);
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var sut = CreateSut(context);
        var actual = await sut.GetAsync(dateFilter: dateFilter);
        actual.Should().HaveCount(10)
            .And.AllSatisfy(item => item.DateCreated.Should().BeOnOrAfter(createdFrom.UtcDateTime));
    }

    [Fact]
    public async Task GetAsync_ReturnsFilteredPage_WhenCreatedToProvided()
    {
        // There are 50 items (with created dates from BaseDate +0 to BaseDate +49 days).
        // The createdTo date is EXCLUSIVE, so if we try to get +0 to +10 there should be 10 results:
        // [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]
        var createdTo = BaseDateTime.AddDays(10);
        var dateFilter = new TimestampFilter(CreatedTo: createdTo.UtcDateTime);
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var sut = CreateSut(context);
        var actual = await sut.GetAsync(dateFilter: dateFilter);
        actual.Should().HaveCount(10)
            .And.AllSatisfy(item => item.DateCreated.Should().BeBefore(createdTo.UtcDateTime));
    }

    [Fact]
    public async Task GetAsync_ReturnsFilteredPage_WhenModifiedFromProvided()
    {
        // There are 50 items (with created dates from BaseDate +0 to BaseDate +49 days).
        // The modifiedFrom date is INCLUSIVE, so if we try to get +45 to +49 there should be 5 results:
        // [45, 46, 47, 48, 49]
        var modifiedFrom = BaseDateTime.AddDays(45).AddYears(1);
        var dateFilter = new TimestampFilter(ModifiedFrom: modifiedFrom.UtcDateTime);
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var sut = CreateSut(context);
        var actual = await sut.GetAsync(dateFilter: dateFilter);
        actual.Should().HaveCount(5)
            .And.AllSatisfy(item => item.DateModified.Should().BeOnOrAfter(modifiedFrom.UtcDateTime));
    }

    [Fact]
    public async Task GetAsync_ReturnsFilteredPage_WhenModifiedToProvided()
    {
        // There are 50 items (with created dates from BaseDate +0 to BaseDate +49 days).
        // The modifiedTo date is EXCLUSIVE, so if we try to get +0 to +5 there should be 5 results:
        // [0, 1, 2, 3, 4]
        var modifiedTo = BaseDateTime.AddDays(5).AddYears(1);
        var dateFilter = new TimestampFilter(ModifiedTo: modifiedTo.UtcDateTime);
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var sut = CreateSut(context);
        var actual = await sut.GetAsync(dateFilter: dateFilter);
        actual.Should().HaveCount(5)
            .And.AllSatisfy(item => item.DateCreated.Should().BeBefore(modifiedTo.UtcDateTime));
    }
    
    /*
     * GetByIdAsync
     */

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenRecordNotFound()
    {
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var sut = CreateSut(context);
        var actual = await sut.GetByIdAsync("does not exist", default);
        actual.Should().BeNull();
    }
    
    [Fact]
    public async Task GetByIdAsync_ReturnsEntity_WhenRecordFound()
    {
        const int itemIndex = 27;
        var id = new Guid($"{itemIndex:D32}").ToString("N");
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var sut = CreateSut(context);
        var actual = await sut.GetByIdAsync(id, default);
        actual.Should().NotBeNull().And.Match<App>(item => item.Id == id);
    }
    
    /*
     * CreateAsync
     */

    [Fact]
    public async Task CreateAsync_AddsEntityToChangeTracker()
    {
        var entity = TestEntityFactory.CreateApp();
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var sut = CreateSut(context);
        _ = await sut.CreateAsync(entity, default);

        context.ChangeTracker.Entries<App>()
            .Where(entry => entry.State == EntityState.Added)
            .Should().HaveCount(1);
    }
    
    /*
     * UpdateAsync
     */
    
    [Fact]
    public async Task UpdateAsync_SetsEntityAsModified_InChangeTracker()
    {
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var entity = context.Apps.Skip(7).First();
        entity.Update("new name");
        
        var sut = CreateSut(context);
        _ = await sut.UpdateAsync(entity, default);

        context.ChangeTracker.Entries<App>()
            .Where(entry => entry.State == EntityState.Modified)
            .Should().HaveCount(1);
    }
    
    /*
     * DeleteAsync
     */
    
    [Fact]
    public async Task DeleteAsync_SetsEntityAsDeleted_InChangeTracker()
    {
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var entity = context.Apps.Skip(12).First();
        
        var sut = CreateSut(context);
        _ = await sut.DeleteAsync(entity, default);

        context.ChangeTracker.Entries<App>()
            .Where(entry => entry.State == EntityState.Deleted)
            .Should().HaveCount(1);
    }
    
    /*
     * Private methods
     */

    private static readonly ICollection<App> SeedData = Enumerable.Range(0, 50).Select(GetEntity).ToList();
    
    private static DateTimeOffset BaseDateTime => new(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
    
    private static AppRepository CreateSut(ProductDbContext context)
        => new(context);
    
    private static async Task PlantSeedDataAsync(ProductDbContext context)
    {
        await context.Apps.AddRangeAsync(SeedData);
        await context.SaveChangesAsync();
    }

    private static App GetEntity(int seed)
    {
        using var guidFixture = new GuidProviderContext(new Guid($"{seed:D32}"));
        using var timeFixture = new DateTimeOffsetProviderContext(BaseDateTime.AddDays(seed));
        return TestEntityFactory.CreateApp($"App {seed:D3}", $"Description of App {seed:D3}", seed % 10 == 0);
    }
}