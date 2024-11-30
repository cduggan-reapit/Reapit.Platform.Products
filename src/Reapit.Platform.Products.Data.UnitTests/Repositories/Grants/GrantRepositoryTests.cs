using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Data.Context;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Repositories.Grants;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Data.UnitTests.Repositories.Grants;

public class GrantRepositoryTests : DatabaseAwareTestBase
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
        var expected = context.Grants.OrderBy(r => r.Cursor).Skip(5).Take(5);
        var cursor = await expected.MinAsync(item => item.Cursor) - 1;
        var ids = await expected.Select(item => item.Id).ToListAsync();
        
        var sut = CreateSut(context);
        var actual = await sut.GetAsync(pagination: new PaginationFilter(cursor, 5));
        actual.Should().HaveCount(5)
            .And.AllSatisfy(item => ids.Should().Contain(item.Id));
    }
    
    [Fact]
    public async Task GetAsync_ReturnsFilteredPage_WhenCreatedFromProvided()
    {
        var createdFrom = BaseDateTime.AddDays(80);
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
        var modifiedFrom = BaseDateTime.AddDays(85).AddYears(1);
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
        var modifiedTo = BaseDateTime.AddDays(5).AddYears(1);
        var dateFilter = new TimestampFilter(ModifiedTo: modifiedTo.UtcDateTime);
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var sut = CreateSut(context);
        var actual = await sut.GetAsync(dateFilter: dateFilter);
        actual.Should().HaveCount(5)
            .And.AllSatisfy(item => item.DateCreated.Should().BeBefore(modifiedTo.UtcDateTime));
    }

    [Fact]
    public async Task GetAsync_ReturnsFilteredPage_WhenClientIdProvided()
    {
        // We re-use client ids for grants since the setup here is 1:1
        var clientId = GetGrantId(3, 29, ClientType.Machine);

        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);

        var sut = CreateSut(context);
        var actual = await sut.GetAsync(clientId: clientId);
        actual.Should().HaveCount(1)
            .And.AllSatisfy(item => item.ClientId.Should().Be(clientId));
    }
    
    [Fact]
    public async Task GetAsync_ReturnsFilteredPage_WhenExternalIdProvided()
    {
        // We re-use client ids for grants since the setup here is 1:1, and prefix them with 'external-'
        var externalId = "external-" + GetGrantId(3, 29, ClientType.Machine);

        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);

        var sut = CreateSut(context);
        var actual = await sut.GetAsync(externalId: externalId);
        actual.Should().HaveCount(1)
            .And.AllSatisfy(item => item.ExternalId.Should().Be(externalId));
    }
    
    [Fact]
    public async Task GetAsync_ReturnsFilteredPage_WhenResourceServerIdProvided()
    {
        // We made 90 grants across 5 resource servers, so there's 18 in each
        const int resourceServerNumber = 3;
        var resourceServerId = $"{resourceServerNumber:D32}";

        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);

        var sut = CreateSut(context);
        var actual = await sut.GetAsync(resourceServerId: resourceServerId);
        actual.Should().HaveCount(18)
            .And.AllSatisfy(item => item.ResourceServerId.Should().Be(resourceServerId));
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
        var id = GetGrantId(4, 37, ClientType.AuthCode);
        
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var sut = CreateSut(context);
        var actual = await sut.GetByIdAsync(id, default);
        actual.Should().NotBeNull().And.Match<Grant>(item => item.Id == id);
    }
    
    /*
     * CreateAsync
     */

    [Fact]
    public async Task CreateAsync_AddsEntityToChangeTracker()
    {
        // This is going to be a little bit of a pain, so bear with me...
        // First we set up the database
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        // Now we grab a client and a resource server that aren't already linked:
        var client = context.Clients.Single(client => client.Id == GetGrantId(0, 3, ClientType.AuthCode));
        var api = context.ResourceServers.Include(rs => rs.Scopes).OrderBy(rs => rs.Id).Last();

        // and create a new grant for them
        var grant = new Grant("new-grant-external-id", client.Id, api.Id)
        {
            Scopes = api.Scopes.Take(2).ToList()
        };

        var sut = CreateSut(context);
        var actual = await sut.CreateAsync(grant, default);
        actual.Should().BeSameAs(grant);

        context.ChangeTracker.Entries<Grant>()
            .Where(entry => entry.State == EntityState.Added)
            .Should().HaveCount(1)
            .And.AllSatisfy(entry => entry.Entity.Scopes.Should().HaveCount(2));
    }
    
    /*
     * UpdateAsync
     */
    
     [Fact]
     public async Task UpdateAsync_SetsEntityAsModified_InChangeTracker()
     {
         // This is going to be a little bit of a pain, so bear with me...
         // First we set up the database
         await using var context = await GetContextAsync();
         await PlantSeedDataAsync(context);
         
         // Then we get a single grant, with all of its gubbins
         var id = GetGrantId(1, 11, ClientType.Machine);
         var grant = await context.Grants
             .Include(grant => grant.Scopes)
             .SingleOrDefaultAsync(grant => grant.Id == id)
             ?? throw new Exception("Failed to get grant");

         // Then we fix time (to avoid buggering up other tests)
         using var timeFixture = new DateTimeOffsetProviderContext(new DateTimeOffset(grant.DateModified, TimeSpan.Zero));
         
         // Then we remove a scope (which will set it as dirty)
         grant.SetScopes(grant.Scopes.Take(1).ToList());
         
         var sut = CreateSut(context);
         _ = await sut.UpdateAsync(grant, default);
         
         context.ChangeTracker.Entries<Grant>()
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
        
        var entity = context.Grants.Skip(12).First();
        
        var sut = CreateSut(context);
        _ = await sut.DeleteAsync(entity, default);

        context.ChangeTracker.Entries<Grant>()
            .Where(entry => entry.State == EntityState.Deleted)
            .Should().HaveCount(1);
    }
    
    /*
     * Private methods
     */
    
    private static DateTimeOffset BaseDateTime => new(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
    
    private static GrantRepository CreateSut(ProductDbContext context)
        => new(context);
    
    private static async Task PlantSeedDataAsync(ProductDbContext context)
    {
        await context.ResourceServers.AddRangeAsync(SeedResourceServers);
        await context.Apps.AddRangeAsync(SeedApps);
        await context.Clients.AddRangeAsync(SeedClients);
        await context.SaveChangesAsync();

        var grants = await GetSeedGrants(context);
        await context.Grants.AddRangeAsync(grants);
        await context.SaveChangesAsync();
    }

    private static ICollection<ResourceServer> SeedResourceServers => Enumerable
        .Range(0, 5)
        .Select(seed =>
        {
            using var guidFixture = new GuidProviderContext(new Guid($"{seed:D32}"));
            using var timeFixture = new DateTimeOffsetProviderContext(BaseDateTime.AddDays(seed));
            // The actual content of the resource server doesn't matter, we just need to know what the Ids are.
            return TestEntityFactory.CreateResourceServer(name: $"{seed:D2}", scopes: 3);
        })
        .ToList();

    private static ICollection<App> SeedApps => Enumerable
        .Range(0, 5)
        .Select(seed =>
        {
            using var guidFixture = new GuidProviderContext(new Guid($"{seed:D32}"));
            using var timeFixture = new DateTimeOffsetProviderContext(BaseDateTime.AddDays(seed));
            return TestEntityFactory.CreateApp($"{seed:D2}");
        })
        .ToList();

    private static ICollection<Client> SeedClients => Enumerable
        .Range(0, 45)
        .SelectMany(seed =>
        {
            // There are 5 apps [0-4].  We'll fill them as we go and use them as id prefixes to check our maths more easily:
            var appId = (int)Math.Floor(seed / 9d);
            
            using var firstTimeFixture = new DateTimeOffsetProviderContext(BaseDateTime.AddDays(seed));
            using var firstGuidFixture = new GuidProviderContext(new Guid($"{appId:D8}-0000-0000-0001-{seed:D12}"));
            var machineClient = TestEntityFactory.CreateClient($"{appId:D32}", name: $"machine-{seed:D2}", type: ClientType.Machine);
            
            using var secondTimeFixture = new DateTimeOffsetProviderContext(BaseDateTime.AddDays(45 + seed));
            using var secondGuidFixture = new GuidProviderContext(new Guid($"{appId:D8}-0000-0000-0002-{seed:D12}"));
            var authCodeClient = TestEntityFactory.CreateClient($"{appId:D32}", name: $"authCode-{seed:D2}", type: ClientType.AuthCode);
                
            return new [] { machineClient, authCodeClient };
        })
        .ToList();

    private static async Task<ICollection<Grant>> GetSeedGrants(ProductDbContext context)
    {
        // For every client, we'll make one grant to the resource server with the same id as the client app (since they
        // are both 0-4).  We'll include the resource server and it's scopes and add only the first two scopes to each
        // grant.
        var apis = await context.ResourceServers.Include(rs => rs.Scopes).ToListAsync();

        var grants = (await context.Clients.ToListAsync())
            .Select(client =>
            {
                var api = apis.Single(api => api.Id == client.AppId);
                using var timeFixture = new DateTimeOffsetProviderContext(new DateTimeOffset(client.DateCreated, TimeSpan.Zero));
                using var guidFixture = new GuidProviderContext(new Guid(client.Id));
                return new Grant($"external-{client.Id}", client.Id, api.Id)
                {
                    Scopes =  api.Scopes.Take(2).ToList(),
                    DateModified = client.DateModified
                };
            })
            .ToList();

        return grants;
    }

    private static string GetGrantId(int resourceProviderNumber, int clientNumber, ClientType clientType)
        => $"{resourceProviderNumber:D8}00000000{(int)clientType:D4}{clientNumber:D12}";
}