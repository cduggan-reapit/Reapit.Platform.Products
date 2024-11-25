using FluentAssertions;
using Reapit.Platform.Products.Data.Helpers;
using Reapit.Platform.Products.Data.UnitTests.TestHelpers;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.UnitTests.Helpers;

public class DummyFilterHelperTests
{
    /*
     * ApplyNameFilter
     */

    [Fact]
    public void ApplyNameFilter_DoesNotApplyFilter_WhenValueIsNull()
    {
        var queryable = GetDummyCollection(10);
        var actual = queryable.ApplyNameFilter(null);
        actual.Should().BeSameAs(queryable);
    }
    
    [Fact]
    public void ApplyNameFilter_AppliesFilter_WhenValueIsNotNull()
    {
        const string value = "test-dummy-006";
        var queryable = GetDummyCollection(10);
        var actual = queryable.ApplyNameFilter(value);
        
        actual.Should().HaveCount(1)
            .And.AllSatisfy(item => item.Name.Should().Be(value));
    }
    
    /*
     * ApplyCreatedFromFilter
     */
    
    [Fact]
    public void ApplyCreatedFromFilter_DoesNotApplyFilter_WhenValueIsNull()
    {
        var queryable = GetDummyCollection(10);
        var actual = queryable.ApplyCreatedFromFilter(null);
        actual.Should().BeSameAs(queryable);
    }
    
    [Fact]
    public void ApplyCreatedFromFilter_AppliesFilter_WhenValueIsNotNull()
    {
        // CreatedFrom is inclusive, so we expect 5, 6, 7, 8, 9, 10 to be returned.
        var value = new DateTime(2020, 1, 5);
        var queryable = GetDummyCollection(10);
        var actual = queryable.ApplyCreatedFromFilter(value);
        
        actual.Should().HaveCount(6)
            .And.AllSatisfy(item => item.DateCreated.Should().BeOnOrAfter(value));
    }
    
    /*
     * ApplyCreatedToFilter
     */
    
    [Fact]
    public void ApplyCreatedToFilter_DoesNotApplyFilter_WhenValueIsNull()
    {
        var queryable = GetDummyCollection(10);
        var actual = queryable.ApplyCreatedToFilter(null);
        actual.Should().BeSameAs(queryable);
    }
    
    [Fact]
    public void ApplyCreatedToFilter_AppliesFilter_WhenValueIsNotNull()
    {
        // CreatedTo is exclusive, so we expect 1, 2, 3, 4 to be returned.
        var value = new DateTime(2020, 1, 5);
        var queryable = GetDummyCollection(10);
        var actual = queryable.ApplyCreatedToFilter(value);
        
        actual.Should().HaveCount(4)
            .And.AllSatisfy(item => item.DateCreated.Should().BeBefore(value));
    }
    
    /*
     * ApplyModifiedFromFilter
     */
    
    [Fact]
    public void ApplyModifiedFromFilter_DoesNotApplyFilter_WhenValueIsNull()
    {
        var queryable = GetDummyCollection(10);
        var actual = queryable.ApplyModifiedFromFilter(null);
        actual.Should().BeSameAs(queryable);
    }
    
    [Fact]
    public void ApplyModifiedFromFilter_AppliesFilter_WhenValueIsNotNull()
    {
        // ModifiedFrom is inclusive, so we expect 5, 6, 7, 8, 9, 10 to be returned.
        var value = new DateTime(2021, 1, 5);
        var queryable = GetDummyCollection(10);
        var actual = queryable.ApplyModifiedFromFilter(value);
        
        actual.Should().HaveCount(6)
            .And.AllSatisfy(item => item.DateModified.Should().BeOnOrAfter(value));
    }
    
    /*
     * ApplyModifiedToFilter
     */
    
    [Fact]
    public void ApplyModifiedToFilter_DoesNotApplyFilter_WhenValueIsNull()
    {
        var queryable = GetDummyCollection(10);
        var actual = queryable.ApplyModifiedToFilter(null);
        actual.Should().BeSameAs(queryable);
    }
    
    [Fact]
    public void ApplyModifiedToFilter_AppliesFilter_WhenValueIsNotNull()
    {
        // ModifiedTo is exclusive, so we expect 1, 2, 3, 4 to be returned.
        var value = new DateTime(2021, 1, 5);
        var queryable = GetDummyCollection(10);
        var actual = queryable.ApplyModifiedToFilter(value);
        
        actual.Should().HaveCount(4)
            .And.AllSatisfy(item => item.DateModified.Should().BeBefore(value));
    }
    
    /*
     * Private methods
     */

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