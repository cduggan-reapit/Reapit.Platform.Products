using System.Text.Json;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Domain.UnitTests.Entities;

public class ResourceServerTests
{
    /*
     * Ctor
     */

    [Fact]
    public void Ctor_SetsProperties_FromParameters()
    {
        // Fix the guid
        var guid = Guid.NewGuid();
        using var guidProvider = new GuidProviderContext(guid);
        
        // Fix the time (it doesn't matter what it's fixed to, we'll refer back to this variable to confirm)
        using var timeProvider = new DateTimeOffsetProviderContext(BaseDate);
        var cursor = (long)(BaseDate - DateTimeOffset.UnixEpoch).TotalMicroseconds;
        
        const string externalId = "external-id", audience = "audience", name = "name";
        const int tokenLifetime = 3600;
        
        var entity = new ResourceServer(externalId, audience, name, tokenLifetime);
        
        // Explicit
        entity.Name.Should().Be(name);
        entity.ExternalId.Should().Be(externalId);
        entity.Audience.Should().Be(audience);
        entity.TokenLifetime.Should().Be(tokenLifetime);
        
        // Implicit
        entity.Id.Should().Be($"{guid:N}");
        entity.Cursor.Should().Be(cursor); 
        entity.DateCreated.Should().Be(BaseDate.UtcDateTime);
        entity.DateModified.Should().Be(BaseDate.UtcDateTime);
    }
    
    /*
     * Update
     */

    [Fact]
    public void Update_DoesNotChangeEntity_WhenValuesNull()
    {
        var entity = GetEntity();
        entity.Update();
        entity.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void Update_DoesNotChangeEntity_WhenValuesUnchanged()
    {
        var entity = GetEntity();
        entity.Update(entity.Name, entity.TokenLifetime);
        entity.IsDirty.Should().BeFalse();
    }

    [Fact]
    public void Update_ModifiesEntity_WhenValuesChanged()
    {
        var entity = GetEntity();
        entity.Update("new name");
        entity.IsDirty.Should().BeTrue();
        entity.DateModified.Should().NotBe(BaseDate.UtcDateTime);
    }
    
    /*
     * SoftDelete
     */

    [Fact]
    public void SoftDelete_SetsDateDeleted_WhenCalled()
    {
        var entity = GetEntity();
        entity.SoftDelete();
        entity.DateDeleted.Should().NotBeNull();
    }
    
    /*
     * AsSerializable
     */

    [Fact]
    public void AsSerializable_CreatesAnonymousObject_WithExpectedProperties()
    {
        var entity = GetEntity();
        var expected = new { entity.Id, entity.Name, entity.DateCreated, entity.DateModified };
        var actual = entity.AsSerializable();
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * ToString
     */

    [Fact]
    public void ToString_ReturnsSerializedObject_FromAsSerializable()
    {
        var entity = GetEntity();
        var expected = JsonSerializer.Serialize(new { entity.Id, entity.Name, entity.DateCreated, entity.DateModified });
        var actual = entity.ToString();
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * Private methods
     */
    
    private static readonly DateTimeOffset BaseDate = new(2024, 11, 29, 13, 18, 17, TimeSpan.Zero);

    private static ResourceServer GetEntity(
        string externalId = "external-id",
        string audience = "audience",
        string name = "name",
        int tokenLifetime = 86400)
    {
        ResourceServer entity;
        using (new DateTimeOffsetProviderContext(BaseDate))
        {
            entity = new ResourceServer(externalId, audience, name, tokenLifetime);
        }
        return entity;
    }
}