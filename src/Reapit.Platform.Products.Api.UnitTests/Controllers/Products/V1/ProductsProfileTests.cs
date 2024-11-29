using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Api.Controllers.Products.V1;
using Reapit.Platform.Products.Api.Controllers.Products.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Api.Extensions;
using Reapit.Platform.Products.Core.UseCases.Products.GetProducts;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Api.UnitTests.Controllers.Products.V1;

public class ProductsProfileTests
{
    /*
     * Product => ProductModel
     */
    
    [Fact]
    public void ProductsProfile_CreatesProductModel_FromProduct()
    {
        var product = GetProduct();
        var expected = new ProductModel(product.Id, product.Name, product.DateCreated, product.DateModified);
        
        var sut = CreateSut();
        var actual = sut.Map<ProductModel>(product);
        actual.Should().BeEquivalentTo(expected);
    } 
    
    /*
     * Product => ProductDetailsModel & [ ProductDetailsClientModel ]
     */
    
    [Fact]
    public void ProductsProfile_CreatesProductDetailsModel_FromProduct()
    {
        var productId = Guid.NewGuid();
        var product = GetProduct(id: productId);
        
        var expected = new ProductDetailsModel(product.Id, product.Name, product.Description, product.DateCreated, product.DateModified);
        
        var sut = CreateSut();
        var actual = sut.Map<ProductDetailsModel>(product);
        actual.Should().BeEquivalentTo(expected);
    } 
    
    /*
     * IEnumerable<Product> => PagedResult<ProductModel>
     */
    
    [Fact]
    public void ProductsProfile_CreatesPagedResult_FromProductCollection()
    {
        const int expectedPageSize = 5;
        var products = Enumerable.Range(0, expectedPageSize).Select(_ => GetProduct()).ToList();

        var sut = CreateSut();
        var expected = new ResultPage<ProductModel>(
            Data: sut.Map<IEnumerable<ProductModel>>(products),
            Cursor: products.GetMaximumCursor(),
            Count: expectedPageSize);

        var actual = sut.Map<ResultPage<ProductModel>>(products);
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * GetProductsRequestModel => GetProductsQuery
     */

    [Fact]
    public void ProductsProfile_PopulatesGetProductsQuery_FromGetProductsRequestModel()
    {
        var request = new GetProductsRequestModel(
            1234,
            72,
            "name",
            "description",
            BaseDate.AddHours(1),
            BaseDate.AddHours(2),
            BaseDate.AddHours(3),
            BaseDate.AddHours(4)
        );

        var expected = new GetProductsQuery(
            request.Cursor, 
            request.PageSize, 
            request.Name, 
            request.Description,
            request.CreatedFrom, 
            request.CreatedTo, 
            request.ModifiedFrom,
            request.ModifiedTo);

        var sut = CreateSut();
        var actual = sut.Map<GetProductsQuery>(request);
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * Private methods
     */
    
    private static readonly DateTime BaseDate = new(2020, 1, 1, 12, 30, 15, DateTimeKind.Utc); 

    private static IMapper CreateSut()
        => new MapperConfiguration(cfg => cfg.AddProfile<ProductsProfile>())
            .CreateMapper();

    private static Product GetProduct(Guid? id = null, string name = "name", string description = "description", DateTime? dateCreated = null, DateTime? dateModified = null)
    {
        // Set values to default if nothing is provided
        id ??= Guid.NewGuid();
        dateCreated ??= DateTime.UtcNow;
        dateModified ??= dateCreated;
        //clients ??= new List<ProductClient>();
        
        using var guidContext = new GuidProviderContext(id.Value);
        using var timeContext = new DateTimeOffsetProviderContext(new DateTimeOffset(dateCreated.Value, TimeSpan.Zero));
        
        return new Product(name, description)
        {
            DateModified = dateModified.Value,
            //Clients = clients
        };
    }
}