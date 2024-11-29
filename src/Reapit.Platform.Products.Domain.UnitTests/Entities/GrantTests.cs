using System.Text.Json;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Domain.UnitTests.Entities;

public class GrantTests
{
    /*
     * Ctor
     */

    [Fact]
    public void Ctor_SetsProperties_FromIdentifiers()
    {
        // Fix the guid
        var guid = Guid.NewGuid();
        using var guidProvider = new GuidProviderContext(guid);
        
        // Fix the time (it doesn't matter what it's fixed to, we'll refer back to this variable to confirm)
        using var timeProvider = new DateTimeOffsetProviderContext(BaseDate);
        var cursor = (long)(BaseDate - DateTimeOffset.UnixEpoch).TotalMicroseconds;
        
        const string externalId = "external-id", 
            clientId = "client-id", 
            resourceServerId = "resource-server-id";
        
        var entity = new Grant(externalId, clientId, resourceServerId);
        
        // Explicit
        entity.ExternalId.Should().Be(externalId);
        entity.ClientId.Should().Be(clientId);
        entity.ResourceServerId.Should().Be(resourceServerId);
        
        // Implicit
        entity.Id.Should().Be($"{guid:N}");
        entity.Cursor.Should().Be(cursor); 
        entity.DateCreated.Should().Be(BaseDate.UtcDateTime);
        entity.DateModified.Should().Be(BaseDate.UtcDateTime);
    }
    
    [Fact]
    public void Ctor_SetsProperties_FromEntities()
    {
        // Fix the guid
        var guid = Guid.NewGuid();
        using var guidProvider = new GuidProviderContext(guid);
        
        // Fix the time (it doesn't matter what it's fixed to, we'll refer back to this variable to confirm)
        using var timeProvider = new DateTimeOffsetProviderContext(BaseDate);
        var cursor = (long)(BaseDate - DateTimeOffset.UnixEpoch).TotalMicroseconds;

        const string externalId = "external-id";
        var client = new Client("", "", ClientType.Machine, "", null, null, null, null);
        var resourceServer = new ResourceServer("", "", "", 3600);
        var scopes = new[] { new Scope(resourceServer.Id, "thing.action", null) };
        
        var entity = new Grant(externalId, client, resourceServer, scopes);
        
        // Explicit
        entity.ExternalId.Should().Be(externalId);
        entity.ClientId.Should().Be(client.Id);
        entity.Client.Should().Be(client);
        entity.ResourceServerId.Should().Be(resourceServer.Id);
        entity.ResourceServer.Should().Be(resourceServer);
        entity.Scopes.Should().BeEquivalentTo(scopes);
        
        // Implicit
        entity.Id.Should().Be($"{guid:N}");
        entity.Cursor.Should().Be(cursor); 
        entity.DateCreated.Should().Be(BaseDate.UtcDateTime);
        entity.DateModified.Should().Be(BaseDate.UtcDateTime);
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
        const string scopeName = "example.scope"; 
        var entity = GetEntity(scopes: [new Scope("", scopeName, null)]);
        var expected = new { entity.Id, entity.ClientId, entity.ResourceServerId, Scopes = new [] { scopeName } };
        var actual = entity.AsSerializable();
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * ToString
     */

    [Fact]
    public void ToString_ReturnsSerializedObject_FromAsSerializable()
    {
        
        const string scopeName = "example.scope"; 
        var entity = GetEntity(scopes: [new Scope("", scopeName, null)]);
        var expected = JsonSerializer.Serialize(new { entity.Id, entity.ClientId, entity.ResourceServerId, Scopes = new [] { scopeName } });
        var actual = entity.ToString();
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * Private methods
     */
    
    private static readonly DateTimeOffset BaseDate = new(2024, 11, 29, 13, 18, 17, TimeSpan.Zero);

    private static Grant GetEntity(
        string externalId = "external-id",
        string clientId = "app-id",
        string resourceServerId = "external-id",
        params Scope[]? scopes)
    {
        Grant entity;
        using (new DateTimeOffsetProviderContext(BaseDate))
        {
            entity = new Grant(externalId, clientId, resourceServerId) { Scopes = scopes ?? [] };
        }
        return entity;
    }
}