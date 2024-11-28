using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Data.Repositories.ProductClients;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Data.UnitTests.Repositories.ProductClients;

public class ProductClientClientFilterHelperTests
{
    /*
     * ApplyCursorFilter
     */

    [Fact]
    public void ApplyCursorFilter_DoesNotApplyFilter_WhenNullProvided()
    {
        var filtered = ProductClients.ApplyCursorFilter(null);
        filtered.Should().HaveCount(200, "there are 200 items in the collection");
    }
    
    [Fact]
    public void ApplyCursorFilter_AppliesFilter_WhenFilterProvided()
    {
        var cursor = (long)(BaseDate.AddDays(49) - DateTimeOffset.UnixEpoch).TotalMicroseconds;
        var filtered = ProductClients.ApplyCursorFilter(cursor);
        filtered.Should().HaveCount(150, "150 items should have a cursor greater than {0}", cursor);
    }
    
    /*
     * ApplyNameFilter
     */
    
    [Fact]
    public void ApplyNameFilter_DoesNotApplyFilter_WhenNullProvided()
    {
        var filtered = ProductClients.ApplyNameFilter(null);
        filtered.Should().HaveCount(200, "there are 200 items in the collection");
    }
    
    [Fact]
    public void ApplyNameFilter_AppliesFilter_WhenFilterProvided()
    {
        const string filter = "ProductClient 047";
        var filtered = ProductClients.ApplyNameFilter(filter);
        filtered.Should().HaveCount(1).And.AllSatisfy(record => record.Name.Should().Be(filter));
    }
    
    /*
     * ApplyDescriptionFilter
     */
    
    [Fact]
    public void ApplyDescriptionFilter_DoesNotApplyFilter_WhenNullProvided()
    {
        var filtered = ProductClients.ApplyDescriptionFilter(null);
        filtered.Should().HaveCount(200, "there are 200 items in the collection");
    }
    
    [Fact]
    public void ApplyDescriptionFilter_AppliesFilter_WhenFilterProvided()
    {
        const string filter = "ProductClient 04";
        var filtered = ProductClients.ApplyDescriptionFilter(filter);
        filtered.Should().HaveCount(10).And.AllSatisfy(record => record.Description.Should().Contain(filter));
    }
    
    /*
     * ApplyProductIdFilter
     */
    
    [Fact]
    public void ApplyProductIdFilter_DoesNotApplyFilter_WhenNullProvided()
    {
        var filtered = ProductClients.ApplyProductIdFilter(null);
        filtered.Should().HaveCount(200);
    }
    
    [Fact]
    public void ApplyProductIdFilter_AppliesFilter_WhenFilterProvided()
    {
        const string filter = "product-6";
        var filtered = ProductClients.ApplyProductIdFilter(filter);
        filtered.Should().HaveCount(20).And.AllSatisfy(record => record.ProductId.Should().Be(filter));
    }
    
    /*
     * ApplyTypeFilter
     */
    
    [Fact]
    public void ApplyTypeFilter_DoesNotApplyFilter_WhenNullProvided()
    {
        var filtered = ProductClients.ApplyTypeFilter(null);
        filtered.Should().HaveCount(200);
    }
    
    [Fact]
    public void ApplyTypeFilter_AppliesFilter_WhenFilterProvided()
    {
        var filter = ClientType.AuthorizationCode;
        var filtered = ProductClients.ApplyTypeFilter(filter);
        filtered.Should().HaveCount(100).And.AllSatisfy(record => record.Type.Should().Be(filter));
    }
    
    /*
     * ApplyCreatedFromFilter
     */
    
    [Fact]
    public void ApplyCreatedFromFilter_DoesNotApplyFilter_WhenNullProvided()
    {
        var filtered = ProductClients.ApplyCreatedFromFilter(null);
        filtered.Should().HaveCount(200, "there are 200 items in the collection");
    }
    
    [Fact]
    public void ApplyCreatedFromFilter_AppliesFilter_WhenFilterProvided()
    {
        var filter = BaseDate.AddDays(99).UtcDateTime;
        var filtered = ProductClients.ApplyCreatedFromFilter(filter);
        filtered.Should().HaveCount(101).And.AllSatisfy(record => record.DateCreated.Should().BeOnOrAfter(filter));
    }
    
    /*
     * ApplyCreatedToFilter
     */
    
    [Fact]
    public void ApplyCreatedToFilter_DoesNotApplyFilter_WhenNullProvided()
    {
        var filtered = ProductClients.ApplyCreatedToFilter(null);
        filtered.Should().HaveCount(200, "there are 200 items in the collection");
    }
    
    [Fact]
    public void ApplyCreatedToFilter_AppliesFilter_WhenFilterProvided()
    {
        var filter = BaseDate.AddDays(49).UtcDateTime;
        var filtered = ProductClients.ApplyCreatedToFilter(filter);
        filtered.Should().HaveCount(49).And.AllSatisfy(record => record.DateCreated.Should().BeBefore(filter));
    }
    
    /*
     * ApplyModifiedFromFilter
     */
    
    [Fact]
    public void ApplyModifiedFromFilter_DoesNotApplyFilter_WhenNullProvided()
    {
        var filtered = ProductClients.ApplyModifiedFromFilter(null);
        filtered.Should().HaveCount(200, "there are 200 items in the collection");
    }
    
    [Fact]
    public void ApplyModifiedFromFilter_AppliesFilter_WhenFilterProvided()
    {
        var filter = BaseDate.AddDays(99).AddYears(1).UtcDateTime;
        var filtered = ProductClients.ApplyModifiedFromFilter(filter);
        filtered.Should().HaveCount(101).And.AllSatisfy(record => record.DateModified.Should().BeOnOrAfter(filter));
    }
    
    /*
     * ApplyModifiedToFilter
     */
    
    [Fact]
    public void ApplyModifiedToFilter_DoesNotApplyFilter_WhenNullProvided()
    {
        var filtered = ProductClients.ApplyModifiedToFilter(null);
        filtered.Should().HaveCount(200, "there are 200 items in the collection");
    }
    
    [Fact]
    public void ApplyModifiedToFilter_AppliesFilter_WhenFilterProvided()
    {
        var filter = BaseDate.AddDays(49).AddYears(1).UtcDateTime;
        var filtered = ProductClients.ApplyModifiedToFilter(filter);
        filtered.Should().HaveCount(49).And.AllSatisfy(record => record.DateModified.Should().BeBefore(filter));
    }

    /*
     * Private methods
     */

    private static readonly DateTimeOffset BaseDate = new(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
    
    private static IQueryable<ProductClient> ProductClients
        => Enumerable.Range(0, 200)
            .Select(i =>
            {
                using var guidContext = new GuidProviderContext(new Guid($"{i:D32}"));
                using var timeContext = new DateTimeOffsetProviderContext(BaseDate.AddDays(i));
                
                var product = i % 10;
                var type = i % 2 == 0 ? ClientType.ClientCredentials : ClientType.AuthorizationCode;
                return new ProductClient(
                    productId: $"product-{product}",
                    clientId: $"client-id-{i:D3}",
                    grantId: $"grant-id-{i:D3}",
                    name: $"ProductClient {i:D3}",
                    description: $"Description of ProductClient {i:D3}",
                    type: type,
                    audience: null,
                    callbackUrls: ArraySegment<string>.Empty, 
                    signOutUrls: ArraySegment<string>.Empty)
                {
                    DateModified = BaseDate.UtcDateTime.AddDays(i).AddYears(1)
                };
            })
            .AsQueryable();
}