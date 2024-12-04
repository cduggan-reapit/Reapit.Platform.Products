using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Data.Repositories.Grants;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Data.UnitTests.Repositories.Grants;

public class GrantFilterHelperTests
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
     * ApplyExternalIdFilter
     */

    [Fact]
    public void ApplyExternalIdFilter_DoesNotApplyFilter_WhenValueNull()
    {
        var data = SeedData;
        var actual = data.ApplyExternalIdFilter(null);
        actual.Should().BeSameAs(data);
    }
    
    [Fact]
    public void ApplyExternalIdFilter_AppliesFilter_WhenValueProvided()
    {
        const string externalId = "external-id-022";
        var data = SeedData;
        var actual = data.ApplyExternalIdFilter(externalId);
        actual.Should().HaveCount(1)
            .And.AllSatisfy(item => item.ExternalId.Should().Be(externalId));
    }
    
    /*
     * ApplyClientIdFilter
     */

    [Fact]
    public void ApplyClientIdFilter_DoesNotApplyFilter_WhenValueNull()
    {
        var data = SeedData;
        var actual = data.ApplyClientIdFilter(null);
        actual.Should().BeSameAs(data);
    }
    
    [Fact]
    public void ApplyClientIdFilter_AppliesFilter_WhenValueProvided()
    {
        const int clientNumber = 8;
        var clientId = $"{clientNumber:D32}";
        var data = SeedData;
        var actual = data.ApplyClientIdFilter(clientId);
        actual.Should().HaveCount(5)
            .And.AllSatisfy(item => item.ClientId.Should().Be(clientId));
    }
    
    /*
     * ApplyResourceServerIdFilter
     */

    [Fact]
    public void ApplyResourceServerIdFilter_DoesNotApplyFilter_WhenValueNull()
    {
        var data = SeedData;
        var actual = data.ApplyResourceServerIdFilter(null);
        actual.Should().BeSameAs(data);
    }
    
    [Fact]
    public void ApplyResourceServerIdFilter_AppliesFilter_WhenValueProvided()
    {
        const int resourceServerNumber = 4;
        var resourceServerId = $"{resourceServerNumber:D32}";
        var data = SeedData;
        var actual = data.ApplyResourceServerIdFilter(resourceServerId);
        actual.Should().HaveCount(10)
            .And.AllSatisfy(item => item.ResourceServerId.Should().Be(resourceServerId));
    }
    
    /*
     * Private methods
     */

    private static readonly DateTime BaseDateTime = new(2020, 1, 1, 0, 0, 0);

    private static IQueryable<Grant> SeedData 
        => Enumerable.Range(0, 50).Select(GetEntity).AsQueryable();

    private static Grant GetEntity(int seed)
    {
        var client = GetClient(seed % 10);
        var resourceServer = GetResourceServer(seed % 5);

        var time = new DateTimeOffset(BaseDateTime, TimeSpan.Zero).AddDays(seed);
        using var guidFixture = new GuidProviderContext(new Guid($"{seed:D32}"));
        using var timeFixture = new DateTimeOffsetProviderContext(time);
        return new Grant($"external-id-{seed:D3}", client.Id, resourceServer.Id)
        {
            DateModified = time.UtcDateTime.AddYears(1),
            Client = client,
            ResourceServer = resourceServer
        };
    }
    
    private static Client GetClient(int seed)
    {
        using var _ = new GuidProviderContext(new Guid($"{seed:D32}"));
        return new Client("", "", ClientType.Machine, "", "", null, null, null);
    }

    private static ResourceServer GetResourceServer(int seed)
    {
        using var _ = new GuidProviderContext(new Guid($"{seed:D32}"));
        return new ResourceServer("", "", "", 3600);
    }
}