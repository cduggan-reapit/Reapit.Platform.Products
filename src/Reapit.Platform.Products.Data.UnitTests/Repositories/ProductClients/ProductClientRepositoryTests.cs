using Microsoft.EntityFrameworkCore;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Data.Context;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Repositories.ProductClients;
using Reapit.Platform.Products.Data.UnitTests.TestHelpers;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Data.UnitTests.Repositories.ProductClients;

public class ProductClientRepositoryTests : DatabaseAwareTestBase
{
    /*
     * GetProductClientsAsync
     */
    
    [Fact]
    public async Task GetProductClientsAsync_ReturnsPagedResult_WhenNoParametersArePassed()
    {
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var expected = ProductClients.Take(25);
        
        var sut = CreateSut(context);
        var actual = await sut.GetProductClientsAsync();
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task GetProductClientsAsync_ReturnsPagedResult_WhenPaginationObjectPassed()
    {
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var expected = ProductClients.Skip(40).Take(5);
        
        // Products are 0-indexed to AddDays 39 = Product[40]
        var cursor = (long)(BaseDate.AddDays(39) - DateTimeOffset.UnixEpoch).TotalMicroseconds;
        
        var sut = CreateSut(context);
        var actual = await sut.GetProductClientsAsync(pagination: new PaginationFilter(cursor, 5));
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task GetProductClientsAsync_ReturnsFilteredResult_WhenNameProvided()
    {
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var expected = new []{ ProductClients.ElementAt(67) };
        
        var sut = CreateSut(context);
        var actual = await sut.GetProductClientsAsync(name: "ProductClient 067");
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task GetProductClientsAsync_ReturnsFilteredResult_WhenDescriptionProvided()
    {
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        var expected = ProductClients.Skip(120).Take(10);
        
        var sut = CreateSut(context);
        var actual = await sut.GetProductClientsAsync(description: "ProductClient 12");
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task GetProductClientsAsync_ReturnsFilteredResult_WhenProductIdProvided()
    {
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);

        const int filterId = 14;
        var filter = $"{filterId:D32}";
        var expected = ProductClients.Where(pc => pc.ProductId == filter);
        
        var sut = CreateSut(context);
        var actual = await sut.GetProductClientsAsync(productId: filter);
        actual.Should().HaveCount(10).And.BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task GetProductClientsAsync_ReturnsFilteredResult_WhenClientIdProvided()
    {
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        const string filter = "client-id-099";
        var expected = ProductClients.Where(pc => pc.ClientId == filter);
        
        var sut = CreateSut(context);
        var actual = await sut.GetProductClientsAsync(clientId: filter);
        actual.Should().HaveCount(1).And.BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task GetProductClientsAsync_ReturnsFilteredResult_WhenGrantIdProvided()
    {
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        const string filter = "grant-id-099";
        var expected = ProductClients.Where(pc => pc.GrantId == filter);
        
        var sut = CreateSut(context);
        var actual = await sut.GetProductClientsAsync(grantId: filter);
        actual.Should().HaveCount(1).And.BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task GetProductClientsAsync_ReturnsFilteredResult_WhenTypeProvided()
    {
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var filter = ClientType.ClientCredentials;
        var expected = ProductClients.Where(pc => pc.Type == filter).OrderBy(pc => pc.Cursor).Take(25);
        
        var sut = CreateSut(context);
        var actual = await sut.GetProductClientsAsync(type: filter);
        actual.Should().HaveCount(25).And.BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task GetProductClientsAsync_ReturnsFilteredResult_WhenCreatedFromProvided()
    {
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var dateFilter = new TimestampFilter(CreatedFrom: BaseDate.AddDays(30).UtcDateTime);
        var expected = ProductClients.Skip(30).Take(25);
        
        var sut = CreateSut(context);
        var actual = await sut.GetProductClientsAsync(dateFilter: dateFilter);
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task GetProductClientsAsync_ReturnsFilteredResult_WhenCreatedToProvided()
    {
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var dateFilter = new TimestampFilter(CreatedTo: BaseDate.AddDays(5).UtcDateTime);
        var expected = ProductClients.Take(5);
        
        var sut = CreateSut(context);
        var actual = await sut.GetProductClientsAsync(dateFilter: dateFilter);
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task GetProductClientsAsync_ReturnsFilteredResult_WhenModifiedFromProvided()
    {
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var dateFilter = new TimestampFilter(ModifiedFrom: BaseDate.AddDays(130).AddYears(1).UtcDateTime);
        var expected = ProductClients.Skip(130).Take(25);
        
        var sut = CreateSut(context);
        var actual = await sut.GetProductClientsAsync(dateFilter: dateFilter);
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task GetProductClientsAsync_ReturnsFilteredResult_WhenModifiedToProvided()
    {
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var dateFilter = new TimestampFilter(ModifiedTo: BaseDate.AddDays(5).AddYears(1).UtcDateTime);
        var expected = ProductClients.Take(5);
        
        var sut = CreateSut(context);
        var actual = await sut.GetProductClientsAsync(dateFilter: dateFilter);
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * GetProductClientByIdAsync
     */

    [Fact]
    public async Task GetProductByIdAsync_ReturnsNull_WhenProductDoesNotExist()
    {
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);

        var sut = CreateSut(context);
        var actual = await sut.GetProductClientByIdAsync("non-existent-id", default);
        actual.Should().BeNull();
    }

    [Fact]
    public async Task GetProductByIdAsync_ReturnsProduct_WhenProductExists()
    {
        const int seed = 37;
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);

        var sut = CreateSut(context);
        var actual = await sut.GetProductClientByIdAsync($"{seed:D32}", default);
        actual.Should().NotBeNull().And.Match<ProductClient>(item => item.Id == $"{seed:D32}");
    }
    
    /*
     * CreateAsync
     */

    [Fact]
    public async Task CreateAsync_RegistersCreation_InChangeTracker()
    {
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        var entity = new ProductClient(Guid .Empty.ToString("N"),"client-id", null, "name", "description", ClientType.None, null, null);
        var sut = CreateSut(context);
        _ = await sut.CreateAsync(entity, default);

        context.ChangeTracker.Entries<ProductClient>().Where(entry => entry.State == EntityState.Added)
            .Should().HaveCount(1);
    }
    
    /*
     * UpdateAsync
     */

    [Fact]
    public async Task UpdateAsync_RegistersModification_InChangeTracker()
    {
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        // Set context to TrackAll so we're not detaching the entity.
        context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;

        const int id = 92;
        var entity = await context.ProductClients.SingleAsync(item => item.Id == $"{id:D32}", cancellationToken: default);
        entity.Update("new name");
        
        var sut = CreateSut(context);
        _ = await sut.UpdateAsync(entity, default);

        context.ChangeTracker.Entries<ProductClient>().Where(entry => entry.State == EntityState.Modified)
            .Should().HaveCount(1);
    }
    
    /*
     * DeleteAsync
     */

    [Fact]
    public async Task DeleteAsync_RegistersModification_InChangeTracker()
    {
        await using var context = await GetContextAsync();
        await PlantSeedDataAsync(context);
        
        // Set context to TrackAll so we're not detaching the entity.
        context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;

        const int id = 163;
        var entity = await context.ProductClients.SingleAsync(item => item.Id == $"{id:D32}", cancellationToken: default);
        
        var sut = CreateSut(context);
        _ = await sut.DeleteAsync(entity, default);

        context.ChangeTracker.Entries<ProductClient>().Where(entry => entry.State == EntityState.Deleted)
            .Should().HaveCount(1);
    }
    
    /*
     * Private methods
     */

    private static readonly DateTimeOffset BaseDate = new(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private static ProductClientRepository CreateSut(ProductDbContext context)
        => new(context);

    private static async Task PlantSeedDataAsync(ProductDbContext context)
    {
        // Set it to AsNoTracking by default to avoid including product in results.
        context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        
        await context.Products.AddRangeAsync(Products);
        await context.ProductClients.AddRangeAsync(ProductClients);
        await context.SaveChangesAsync();
    }
    
    private static ICollection<Product> Products
        => Enumerable.Range(0, 20)
            .Select(i =>
            {
                using var guidContext = new GuidProviderContext(new Guid($"{i:D32}"));
                using var timeContext = new DateTimeOffsetProviderContext(BaseDate.AddDays(i));
                return new Product($"Product {i:D3}", $"Description of Product {i:D3}")
                {
                    DateModified = BaseDate.UtcDateTime.AddDays(i).AddYears(1)
                };
            }).ToList();
    
    private static ICollection<ProductClient> ProductClients
        => Enumerable.Range(0, 200)
            .Select(i =>
            {
                var productId = $"{i % 20:D32}";
                var type = i % 2 == 0 ? ClientType.ClientCredentials : ClientType.AuthorizationCode;
                using var guidContext = new GuidProviderContext(new Guid($"{i:D32}"));
                using var timeContext = new DateTimeOffsetProviderContext(BaseDate.AddDays(i));
                return new ProductClient(
                    productId: productId,
                    clientId: $"client-id-{i:D3}",
                    grantId: $"grant-id-{i:D3}",
                    name: $"ProductClient {i:D3}", 
                    description: $"Description of ProductClient {i:D3}",
                    type: type,
                    callbackUrls: null,
                    signOutUrls: null)
                {
                    DateModified = BaseDate.UtcDateTime.AddDays(i).AddYears(1)
                };
            }).ToList();
}