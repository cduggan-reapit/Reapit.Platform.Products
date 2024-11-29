using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Data.Context;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Repositories.Clients;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Data.UnitTests.Repositories.Clients;

public class ClientRepositoryTests : DatabaseAwareTestBase
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
        var expected = context.Clients.OrderBy(r => r.Cursor).Skip(5).Take(5);
        var cursor = await expected.MinAsync(item => item.Cursor) - 1;
        var ids = await expected.Select(item => item.Id).ToListAsync();
        
        var sut = CreateSut(context);
        var actual = await sut.GetAsync(pagination: new PaginationFilter(cursor, 5));
        actual.Should().HaveCount(5)
            .And.AllSatisfy(item => ids.Should().Contain(item.Id));
    }
    
    // TODO: ExternalId
    
    [Fact]
    public async Task GetAsync_ReturnsFilteredPage_WhenAppIdProvided()
    {
        const int appNumber = 2;
        var appId = $"{appNumber:D32}";
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var sut = CreateSut(context);
        var actual = await sut.GetAsync(appId: appId);
        actual.Should().HaveCount(5)
            .And.AllSatisfy(item => item.AppId.Should().Be(appId));
    }
    
    [Fact]
    public async Task GetAsync_ReturnsFilteredPage_WhenTypeProvided()
    {
        var type = ClientType.AuthCode;
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var sut = CreateSut(context);
        var actual = await sut.GetAsync(type: type);
        actual.Should().HaveCount(25)
            .And.AllSatisfy(item => item.Type.Should().Be(type));
    }
    
    [Fact]
    public async Task GetAsync_ReturnsFilteredPage_WhenExternalIdProvided()
    {
        const string externalId = "external-id-049";
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var sut = CreateSut(context);
        var actual = await sut.GetAsync(externalId: externalId);
        actual.Should().HaveCount(1)
            .And.AllSatisfy(item => item.ExternalId.Should().Be(externalId));
    }

    [Fact]
    public async Task GetAsync_ReturnsFilteredPage_WhenNameProvided()
    {
        const string name = "Client 042";
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
        const string description = "Client 03";
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
        actual.Should().NotBeNull().And.Match<Client>(item => item.Id == id);
    }
    
    /*
     * CreateAsync
     */

    [Fact]
    public async Task CreateAsync_AddsEntityToChangeTracker()
    {
        const int appId = 3;
        var entity = TestEntityFactory.CreateClient($"{appId:D32}");
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var sut = CreateSut(context);
        _ = await sut.CreateAsync(entity, default);

        context.ChangeTracker.Entries<Client>()
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
        
        
        var entity = context.Clients.Skip(7).First();
        
        using var timeProvider = new DateTimeOffsetProviderContext(new DateTimeOffset(entity.DateModified, TimeSpan.Zero));
        entity.Update("new name");
        
        var sut = CreateSut(context);
        _ = await sut.UpdateAsync(entity, default);

        context.ChangeTracker.Entries<Client>()
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
        
        var entity = context.Clients.Skip(12).First();
        
        var sut = CreateSut(context);
        _ = await sut.DeleteAsync(entity, default);

        context.ChangeTracker.Entries<Client>()
            .Where(entry => entry.State == EntityState.Deleted)
            .Should().HaveCount(1);
    }
    
    /*
     * Private methods
     */

    private static readonly ICollection<App> SeedApps = Enumerable.Range(0, 10)
        .Select(seed =>
        {
            using var guidFixture = new GuidProviderContext(new Guid($"{seed:D32}"));
            using var timeFixture = new DateTimeOffsetProviderContext(BaseDateTime.AddDays(seed));
            return TestEntityFactory.CreateApp(); 
        })
        .ToList();
    
    private static readonly ICollection<Client> SeedData = Enumerable.Range(0, 50).Select(GetEntity).ToList();
    
    private static DateTimeOffset BaseDateTime => new(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
    
    private static ClientRepository CreateSut(ProductDbContext context)
        => new(context);

    private static async Task PlantSeedDataAsync(ProductDbContext context)
    {
        await context.Apps.AddRangeAsync(SeedApps);
        await context.Clients.AddRangeAsync(SeedData);
        await context.SaveChangesAsync();
    }

    private static Client GetEntity(int seed)
    {
        using var guidFixture = new GuidProviderContext(new Guid($"{seed:D32}"));
        using var timeFixture = new DateTimeOffsetProviderContext(BaseDateTime.AddDays(seed));
        
        // We're generating 50 clients, spread 5 per app for the 10 seed apps:
        var appId = $"{seed%10:D32}";
        var type = seed % 2 == 0 ? ClientType.Machine : ClientType.AuthCode;

        // We only need to populate filterable properties here
        return TestEntityFactory.CreateClient(appId, $"external-id-{seed:D3}", type, $"Client {seed:D3}", $"Description of Client {seed:D3}");
    }
}