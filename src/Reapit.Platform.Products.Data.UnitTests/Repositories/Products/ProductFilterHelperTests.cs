using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Data.Repositories.Products;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.UnitTests.Repositories.Products;

public class ProductFilterHelperTests
{
    /*
     * ApplyCursorFilter
     */

    [Fact]
    public void ApplyCursorFilter_DoesNotApplyFilter_WhenNullProvided()
    {
        var filtered = Products.ApplyCursorFilter(null);
        filtered.Should().HaveCount(200, "there are 200 items in the product collection");
    }
    
    [Fact]
    public void ApplyCursorFilter_AppliesFilter_WhenFilterProvided()
    {
        var cursor = (long)(BaseDate.AddDays(49) - DateTimeOffset.UnixEpoch).TotalMicroseconds;
        var filtered = Products.ApplyCursorFilter(cursor);
        filtered.Should().HaveCount(150, "150 items should have a cursor greater than {0}", cursor);
    }
    
    /*
     * ApplyNameFilter
     */
    
    [Fact]
    public void ApplyNameFilter_DoesNotApplyFilter_WhenNullProvided()
    {
        var filtered = Products.ApplyNameFilter(null);
        filtered.Should().HaveCount(200, "there are 200 items in the product collection");
    }
    
    [Fact]
    public void ApplyNameFilter_AppliesFilter_WhenFilterProvided()
    {
        const string filter = "Product 047";
        var filtered = Products.ApplyNameFilter(filter);
        filtered.Should().HaveCount(1).And.AllSatisfy(record => record.Name.Should().Be(filter));
    }
    
    /*
     * ApplyDescriptionFilter
     */
    
    [Fact]
    public void ApplyDescriptionFilter_DoesNotApplyFilter_WhenNullProvided()
    {
        var filtered = Products.ApplyDescriptionFilter(null);
        filtered.Should().HaveCount(200, "there are 200 items in the product collection");
    }
    
    [Fact]
    public void ApplyDescriptionFilter_AppliesFilter_WhenFilterProvided()
    {
        const string filter = "Product 04";
        var filtered = Products.ApplyDescriptionFilter(filter);
        filtered.Should().HaveCount(10).And.AllSatisfy(record => record.Description.Should().Contain(filter));
    }
    
    /*
     * ApplyCreatedFromFilter
     */
    
    [Fact]
    public void ApplyCreatedFromFilter_DoesNotApplyFilter_WhenNullProvided()
    {
        var filtered = Products.ApplyCreatedFromFilter(null);
        filtered.Should().HaveCount(200, "there are 200 items in the product collection");
    }
    
    [Fact]
    public void ApplyCreatedFromFilter_AppliesFilter_WhenFilterProvided()
    {
        var filter = BaseDate.AddDays(99).UtcDateTime;
        var filtered = Products.ApplyCreatedFromFilter(filter);
        filtered.Should().HaveCount(101).And.AllSatisfy(record => record.DateCreated.Should().BeOnOrAfter(filter));
    }
    
    /*
     * ApplyCreatedToFilter
     */
    
    [Fact]
    public void ApplyCreatedToFilter_DoesNotApplyFilter_WhenNullProvided()
    {
        var filtered = Products.ApplyCreatedToFilter(null);
        filtered.Should().HaveCount(200, "there are 200 items in the product collection");
    }
    
    [Fact]
    public void ApplyCreatedToFilter_AppliesFilter_WhenFilterProvided()
    {
        var filter = BaseDate.AddDays(49).UtcDateTime;
        var filtered = Products.ApplyCreatedToFilter(filter);
        filtered.Should().HaveCount(49).And.AllSatisfy(record => record.DateCreated.Should().BeBefore(filter));
    }
    
    /*
     * ApplyModifiedFromFilter
     */
    
    [Fact]
    public void ApplyModifiedFromFilter_DoesNotApplyFilter_WhenNullProvided()
    {
        var filtered = Products.ApplyModifiedFromFilter(null);
        filtered.Should().HaveCount(200, "there are 200 items in the product collection");
    }
    
    [Fact]
    public void ApplyModifiedFromFilter_AppliesFilter_WhenFilterProvided()
    {
        var filter = BaseDate.AddDays(99).AddYears(1).UtcDateTime;
        var filtered = Products.ApplyModifiedFromFilter(filter);
        filtered.Should().HaveCount(101).And.AllSatisfy(record => record.DateModified.Should().BeOnOrAfter(filter));
    }
    
    /*
     * ApplyModifiedToFilter
     */
    
    [Fact]
    public void ApplyModifiedToFilter_DoesNotApplyFilter_WhenNullProvided()
    {
        var filtered = Products.ApplyModifiedToFilter(null);
        filtered.Should().HaveCount(200, "there are 200 items in the product collection");
    }
    
    [Fact]
    public void ApplyModifiedToFilter_AppliesFilter_WhenFilterProvided()
    {
        var filter = BaseDate.AddDays(49).AddYears(1).UtcDateTime;
        var filtered = Products.ApplyModifiedToFilter(filter);
        filtered.Should().HaveCount(49).And.AllSatisfy(record => record.DateModified.Should().BeBefore(filter));
    }

    /*
     * Private methods
     */

    private static readonly DateTimeOffset BaseDate = new(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);
    
    private static IQueryable<Product> Products
        => Enumerable.Range(0, 200)
            .Select(i =>
            {
                using var guidContext = new GuidProviderContext(new Guid($"{i:D32}"));
                using var timeContext = new DateTimeOffsetProviderContext(BaseDate.AddDays(i));
                return new Product($"Product {i:D3}", $"Description of Product {i:D3}")
                {
                    DateModified = BaseDate.UtcDateTime.AddDays(i).AddYears(1)
                };
            })
            .AsQueryable();
}