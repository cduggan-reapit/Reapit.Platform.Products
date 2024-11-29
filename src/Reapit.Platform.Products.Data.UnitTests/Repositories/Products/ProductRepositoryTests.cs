// using Microsoft.EntityFrameworkCore;
// using Reapit.Platform.Common.Providers.Identifiers;
// using Reapit.Platform.Common.Providers.Temporal;
// using Reapit.Platform.Products.Data.Context;
// using Reapit.Platform.Products.Data.Repositories;
// using Reapit.Platform.Products.Data.Repositories.V0.Products;
// using Reapit.Platform.Products.Data.UnitTests.TestHelpers;
// using Reapit.Platform.Products.Domain.Entities;
// using Reapit.Platform.Products.Domain.Entities.Initial;
//
// namespace Reapit.Platform.Products.Data.UnitTests.Repositories.Products;
//
// public class ProductRepositoryTests : DatabaseAwareTestBase
// {
//     /*
//      * GetProductsAsync
//      */
//
//     [Fact]
//     public async Task GetProductsAsync_ReturnsPagedResult_WhenNoParametersArePassed()
//     {
//         await using var context = await GetContextAsync();
//         await PlantSeedDataAsync(context);
//         var expected = Products.Take(25);
//         
//         var sut = CreateSut(context);
//         var actual = await sut.GetProductsAsync();
//         actual.Should().BeEquivalentTo(expected);
//     }
//     
//     [Fact]
//     public async Task GetProductsAsync_ReturnsPagedResult_WhenPaginationObjectPassed()
//     {
//         await using var context = await GetContextAsync();
//         await PlantSeedDataAsync(context);
//         var expected = Products.Skip(40).Take(5);
//         
//         // Products are 0-indexed to AddDays 39 = Product[40]
//         var cursor = (long)(BaseDate.AddDays(39) - DateTimeOffset.UnixEpoch).TotalMicroseconds;
//         
//         var sut = CreateSut(context);
//         var actual = await sut.GetProductsAsync(pagination: new PaginationFilter(cursor, 5));
//         actual.Should().BeEquivalentTo(expected);
//     }
//     
//     [Fact]
//     public async Task GetProductsAsync_ReturnsFilteredResult_WhenNameProvided()
//     {
//         await using var context = await GetContextAsync();
//         await PlantSeedDataAsync(context);
//         var expected =new []{ Products.ElementAt(67) };
//         
//         var sut = CreateSut(context);
//         var actual = await sut.GetProductsAsync(name: "Product 067");
//         actual.Should().BeEquivalentTo(expected);
//     }
//     
//     [Fact]
//     public async Task GetProductsAsync_ReturnsFilteredResult_WhenDescriptionProvided()
//     {
//         await using var context = await GetContextAsync();
//         await PlantSeedDataAsync(context);
//         // Products collection is zero-indexed so item 120 has id 119.   
//         var expected = Products.Skip(120).Take(10);
//         
//         var sut = CreateSut(context);
//         var actual = await sut.GetProductsAsync(description: "Product 12");
//         actual.Should().BeEquivalentTo(expected);
//     }
//     
//     [Fact]
//     public async Task GetProductsAsync_ReturnsFilteredResult_WhenCreatedFromProvided()
//     {
//         await using var context = await GetContextAsync();
//         await PlantSeedDataAsync(context);
//         
//         var dateFilter = new TimestampFilter(CreatedFrom: BaseDate.AddDays(30).UtcDateTime);
//         var expected = Products.Skip(30).Take(25);
//         
//         var sut = CreateSut(context);
//         var actual = await sut.GetProductsAsync(dateFilter: dateFilter);
//         actual.Should().BeEquivalentTo(expected);
//     }
//     
//     [Fact]
//     public async Task GetProductsAsync_ReturnsFilteredResult_WhenCreatedToProvided()
//     {
//         await using var context = await GetContextAsync();
//         await PlantSeedDataAsync(context);
//         
//         var dateFilter = new TimestampFilter(CreatedTo: BaseDate.AddDays(5).UtcDateTime);
//         var expected = Products.Take(5);
//         
//         var sut = CreateSut(context);
//         var actual = await sut.GetProductsAsync(dateFilter: dateFilter);
//         actual.Should().BeEquivalentTo(expected);
//     }
//     
//     [Fact]
//     public async Task GetProductsAsync_ReturnsFilteredResult_WhenModifiedFromProvided()
//     {
//         await using var context = await GetContextAsync();
//         await PlantSeedDataAsync(context);
//         
//         var dateFilter = new TimestampFilter(ModifiedFrom: BaseDate.AddDays(130).AddYears(1).UtcDateTime);
//         var expected = Products.Skip(130).Take(25);
//         
//         var sut = CreateSut(context);
//         var actual = await sut.GetProductsAsync(dateFilter: dateFilter);
//         actual.Should().BeEquivalentTo(expected);
//     }
//     
//     [Fact]
//     public async Task GetProductsAsync_ReturnsFilteredResult_WhenModifiedToProvided()
//     {
//         await using var context = await GetContextAsync();
//         await PlantSeedDataAsync(context);
//         
//         var dateFilter = new TimestampFilter(ModifiedTo: BaseDate.AddDays(5).AddYears(1).UtcDateTime);
//         var expected = Products.Take(5);
//         
//         var sut = CreateSut(context);
//         var actual = await sut.GetProductsAsync(dateFilter: dateFilter);
//         actual.Should().BeEquivalentTo(expected);
//     }
//     
//     /*
//      * GetProductByIdAsync
//      */
//
//     [Fact]
//     public async Task GetProductByIdAsync_ReturnsNull_WhenProductDoesNotExist()
//     {
//         await using var context = await GetContextAsync();
//         await PlantSeedDataAsync(context);
//
//         var sut = CreateSut(context);
//         var actual = await sut.GetProductByIdAsync("non-existent-id", default);
//         actual.Should().BeNull();
//     }
//
//     [Fact]
//     public async Task GetProductByIdAsync_ReturnsProduct_WhenProductExists()
//     {
//         const int seed = 37;
//         await using var context = await GetContextAsync();
//         await PlantSeedDataAsync(context);
//
//         var sut = CreateSut(context);
//         var actual = await sut.GetProductByIdAsync($"{seed:D32}", default);
//         actual.Should().NotBeNull().And.Match<Product>(item => item.Id == $"{seed:D32}");
//     }
//     
//     /*
//      * CreateAsync
//      */
//
//     [Fact]
//     public async Task CreateAsync_RegistersCreation_InChangeTracker()
//     {
//         await using var context = await GetContextAsync();
//         await PlantSeedDataAsync(context);
//         
//         var entity = new Product("name", "description");
//         var sut = CreateSut(context);
//         _ = await sut.CreateAsync(entity, default);
//
//         context.ChangeTracker.Entries<Product>().Where(entry => entry.State == EntityState.Added)
//             .Should().HaveCount(1);
//     }
//     
//     /*
//      * UpdateAsync
//      */
//
//     [Fact]
//     public async Task UpdateAsync_RegistersModification_InChangeTracker()
//     {
//         await using var context = await GetContextAsync();
//         await PlantSeedDataAsync(context);
//
//         const int id = 92;
//         var entity = await context.Products.SingleAsync(item => item.Id == $"{id:D32}", cancellationToken: default);
//         entity.Update("new name", null);
//         
//         var sut = CreateSut(context);
//         _ = await sut.UpdateAsync(entity, default);
//
//         context.ChangeTracker.Entries<Product>().Where(entry => entry.State == EntityState.Modified)
//             .Should().HaveCount(1);
//     }
//     
//     /*
//      * DeleteAsync
//      */
//
//     [Fact]
//     public async Task DeleteAsync_RegistersModification_InChangeTracker()
//     {
//         await using var context = await GetContextAsync();
//         await PlantSeedDataAsync(context);
//
//         const int id = 163;
//         var entity = await context.Products.SingleAsync(item => item.Id == $"{id:D32}", cancellationToken: default);
//         
//         var sut = CreateSut(context);
//         _ = await sut.DeleteAsync(entity, default);
//
//         context.ChangeTracker.Entries<Product>().Where(entry => entry.State == EntityState.Deleted)
//             .Should().HaveCount(1);
//     }
//     
//     /*
//      * Private methods
//      */
//
//     private static readonly DateTimeOffset BaseDate = new(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
//
//     private static ProductRepository CreateSut(ProductDbContext context)
//         => new(context);
//
//     private static async Task PlantSeedDataAsync(ProductDbContext context)
//     {
//         await context.Products.AddRangeAsync(Products);
//         await context.SaveChangesAsync();
//     }
//     
//     private static IEnumerable<Product> Products
//         => Enumerable.Range(0, 200)
//             .Select(i =>
//             {
//                 using var guidContext = new GuidProviderContext(new Guid($"{i:D32}"));
//                 using var timeContext = new DateTimeOffsetProviderContext(BaseDate.AddDays(i));
//                 return new Product($"Product {i:D3}", $"Description of Product {i:D3}")
//                 {
//                     DateModified = BaseDate.UtcDateTime.AddDays(i).AddYears(1)
//                 };
//             });
// }