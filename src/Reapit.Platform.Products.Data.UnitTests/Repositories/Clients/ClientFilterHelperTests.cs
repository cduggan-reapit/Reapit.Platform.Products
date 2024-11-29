using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Data.Repositories.Clients;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Data.UnitTests.Repositories.Clients;

public class ClientFilterHelperTests
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
     * ApplyAppIdFilter
     */
    
    [Fact]
    public void ApplyAppIdFilter_DoesNotApplyFilter_WhenValueIsNull()
    {
        var data = SeedData;
        var actual = data.ApplyAppIdFilter(null);
        actual.Should().BeSameAs(data);
    }
    
    [Fact]
    public void ApplyAppIdFilter_AppliesFilter_WhenValueProvided()
    {
        const string appId = "app-002";
        var data = SeedData;
        var actual = data.ApplyAppIdFilter(appId);
        actual.Should().HaveCount(5)
            .And.AllSatisfy(item => item.AppId.Should().Be(appId));
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
        const string name = "Client 042";
        var data = SeedData;
        var actual = data.ApplyNameFilter(name);
        actual.Should().HaveCount(1);
    }
    
    /*
     * ApplyDescriptionFilter
     */
    
    [Fact]
    public void ApplyDescriptionFilter_DoesNotApplyFilter_WhenValueIsNull()
    {
        var data = SeedData;
        var actual = data.ApplyDescriptionFilter(null);
        actual.Should().BeSameAs(data);
    }
    
    [Fact]
    public void ApplyDescriptionFilter_AppliesFilter_WhenValueProvided()
    {
        const string description = "042";
        var data = SeedData;
        var actual = data.ApplyDescriptionFilter(description);
        actual.Should().HaveCount(1);
    }
    
    /*
     * ApplyExternalIdFilter
     */
    
    [Fact]
    public void ApplyExternalIdFilter_DoesNotApplyFilter_WhenValueIsNull()
    {
        var data = SeedData;
        var actual = data.ApplyExternalIdFilter(null);
        actual.Should().BeSameAs(data);
    }
    
    [Fact]
    public void ApplyExternalIdFilter_AppliesFilter_WhenValueProvided()
    {
        const string externalId = "external-id-026";
        var data = SeedData;
        var actual = data.ApplyExternalIdFilter(externalId);
        actual.Should().HaveCount(1)
            .And.AllSatisfy(item => item.ExternalId.Should().Be(externalId));
    }
    
    /*
     * ApplyTypeFilter
     */
    
    [Fact]
    public void ApplyTypeFilter_DoesNotApplyFilter_WhenValueIsNull()
    {
        var data = SeedData;
        var actual = data.ApplyTypeFilter(null);
        actual.Should().BeSameAs(data);
    }
    
    [Fact]
    public void ApplyTypeFilter_AppliesFilter_WhenValueProvided()
    {
        var type = ClientType.AuthCode;
        var data = SeedData;
        var actual = data.ApplyTypeFilter(type);
        
        actual.Should().HaveCount(25)
            .And.AllSatisfy(item => item.Type.Should().Be(type));
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

    private static IQueryable<Client> SeedData 
        => Enumerable.Range(0, 50).Select(GetEntity).AsQueryable();

    private static Client GetEntity(int seed)
    {
        using var guidFixture = new GuidProviderContext(new Guid($"{seed:D32}"));
        using var timeFixture = new DateTimeOffsetProviderContext(new DateTimeOffset(BaseDateTime, TimeSpan.Zero).AddDays(seed));
        
        // We're generating 50 clients, lets do 5 per app:
        var appId = $"app-{seed%10:D3}";
        var type = seed % 2 == 0 ? ClientType.Machine : ClientType.AuthCode;

        // We only need to populate filterable properties here
        return TestEntityFactory.CreateClient(appId, $"external-id-{seed:D3}", type, $"Client {seed:D3}", $"Description of Client {seed:D3}");
    }
}