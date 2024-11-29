using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Data.Repositories.Apps;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Data.UnitTests.Repositories.Apps;

public class AppFilterHelperTests
{
    /*
     * ApplyCursor
     */

    [Fact]
    public void ApplyCursor_DoesNotApplyFilter_WhenCursorIsNull()
    {
        var data = SeedData;
        var actual = data.ApplyCursorFilter(null);
        actual.Should().BeSameAs(data);
    }
    
    [Fact]
    public void ApplyCursor_AppliesFilter_WhenCursorProvided()
    {
        // There should be 60 records with a cursor greater than this:
        var cursorTime = new DateTimeOffset(BaseDateTime, TimeSpan.Zero).AddDays(24);
        var cursor = (long)(cursorTime - DateTimeOffset.UnixEpoch).TotalMicroseconds;
        
        var data = SeedData;
        var actual = data.ApplyCursorFilter(cursor);
        actual.Should().HaveCount(25);
    }
    
    /*
     * ApplyNameFilter
     */
    
    [Fact]
    public void ApplyNameFilter_DoesNotApplyFilter_WhenNameIsNull()
    {
        var data = SeedData;
        var actual = data.ApplyNameFilter(null);
        actual.Should().BeSameAs(data);
    }
    
    [Fact]
    public void ApplyNameFilter_AppliesFilter_WhenNameProvided()
    {
        const string name = "App 042";
        var data = SeedData;
        var actual = data.ApplyNameFilter(name);
        actual.Should().HaveCount(1);
    }
    
    /*
     * ApplyDescriptionFilter
     */
    
    [Fact]
    public void ApplyDescriptionFilter_DoesNotApplyFilter_WhenDescriptionIsNull()
    {
        var data = SeedData;
        var actual = data.ApplyDescriptionFilter(null);
        actual.Should().BeSameAs(data);
    }
    
    [Fact]
    public void ApplyDescriptionFilter_AppliesFilter_WhenDescriptionProvided()
    {
        const string description = "042";
        var data = SeedData;
        var actual = data.ApplyDescriptionFilter(description);
        actual.Should().HaveCount(1);
    }
    
    /*
     * ApplySkipConsentFilter
     */
    
    [Fact]
    public void ApplySkipConsentFilter_DoesNotApplyFilter_WhenDescriptionIsNull()
    {
        var data = SeedData;
        var actual = data.ApplySkipConsentFilter(null);
        actual.Should().BeSameAs(data);
    }
    
    [Fact]
    public void ApplySkipConsentFilter_AppliesFilter_WhenDescriptionProvided()
    {
        var data = SeedData;
        var actual = data.ApplySkipConsentFilter(true);
        actual.Should().HaveCount(5, "one in ten apps are marked skipConsent");
    }
    
    /*
     * ApplyCreatedFromFilter
     */
    
    [Fact]
    public void ApplyCreatedFromFilter_DoesNotApplyFilter_WhenCreatedFromIsNull()
    {
        var data = SeedData;
        var actual = data.ApplyCreatedFromFilter(null);
        actual.Should().BeSameAs(data);
    }
    
    [Fact]
    public void ApplyCreatedFromFilter_AppliesFilter_WhenCreatedFromProvided()
    {
        var createdFrom = BaseDateTime.AddDays(24);
        
        var data = SeedData;
        var actual = data.ApplyCreatedFromFilter(createdFrom);
        actual.Should().HaveCount(26, "CreatedFrom is inclusive");
    }
    
    /*
     * ApplyCreatedToFilter
     */
    
    [Fact]
    public void ApplyCreatedToFilter_DoesNotApplyFilter_WhenCreatedToIsNull()
    {
        var data = SeedData;
        var actual = data.ApplyCreatedToFilter(null);
        actual.Should().BeSameAs(data);
    }
    
    [Fact]
    public void ApplyCreatedToFilter_AppliesFilter_WhenCreatedToProvided()
    {
        var createdTo = BaseDateTime.AddDays(24);
        
        var data = SeedData;
        var actual = data.ApplyCreatedToFilter(createdTo);
        actual.Should().HaveCount(24, "CreatedTo is exclusive");
    }
    
    /*
     * ApplyModifiedFromFilter
     */
    
    [Fact]
    public void ApplyModifiedFromFilter_DoesNotApplyFilter_WhenModifiedFromIsNull()
    {
        var data = SeedData;
        var actual = data.ApplyModifiedFromFilter(null);
        actual.Should().BeSameAs(data);
    }
    
    [Fact]
    public void ApplyModifiedFromFilter_AppliesFilter_WhenModifiedFromProvided()
    {
        var modifiedFrom = BaseDateTime.AddDays(24).AddYears(1);
        
        var data = SeedData;
        var actual = data.ApplyModifiedFromFilter(modifiedFrom);
        actual.Should().HaveCount(26, "ModifiedTo is inclusive");
    }
    
    /*
     * ApplyModifiedToFilter
     */
    
    [Fact]
    public void ApplyModifiedToFilter_DoesNotApplyFilter_WhenModifiedToIsNull()
    {
        var data = SeedData;
        var actual = data.ApplyModifiedToFilter(null);
        actual.Should().BeSameAs(data);
    }
    
    [Fact]
    public void ApplyModifiedToFilter_AppliesFilter_WhenModifiedToProvided()
    {
        var modifiedTo = BaseDateTime.AddDays(24).AddYears(1);
        
        var data = SeedData;
        var actual = data.ApplyModifiedToFilter(modifiedTo);
        actual.Should().HaveCount(24, "ModifiedTo is exclusive");
    }
    
    /*
     * Private methods
     */

    private static readonly DateTime BaseDateTime = new(2020, 1, 1, 0, 0, 0);

    private static IQueryable<App> SeedData 
        => Enumerable.Range(0, 50).Select(GetEntity).AsQueryable();

    private static App GetEntity(int seed)
    {
        using var guidFixture = new GuidProviderContext(new Guid($"{seed:D32}"));
        using var timeFixture = new DateTimeOffsetProviderContext(new DateTimeOffset(BaseDateTime, TimeSpan.Zero).AddDays(seed));
        return TestEntityFactory.CreateApp($"App {seed:D3}", $"Description of App {seed:D3}", seed % 10 == 0);
    }
}