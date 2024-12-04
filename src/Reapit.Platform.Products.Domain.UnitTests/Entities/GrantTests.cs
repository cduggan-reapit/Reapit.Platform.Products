using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Reapit.Platform.Products.Domain.UnitTests.Entities;

public class GrantTests
{
    /*
     * Ctor
     */
    
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
        
        var entity = new Grant(externalId, client.Id, resourceServer.Id)
        {
            Client = default!,
            ResourceServer = default!,
            Scopes = scopes
        };
        
        // Explicit
        entity.ExternalId.Should().Be(externalId);
        entity.ClientId.Should().Be(client.Id);
        entity.ResourceServerId.Should().Be(resourceServer.Id);
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
     * SetScopes
     */

    [Fact]
    public void SetScopes_DoesNotModifyEntity_WhenScopesUnchanged()
    {
        var initialScopes = new[]
        {
            new Scope("grants can", "scope.one", null),
            new Scope("only reference", "scope.two", null),
            new Scope("one api", "scope.three", null)
        };
        
        var updatedScopes = new[]
        {
            new Scope("so the only", "scope.ONE", null),
            new Scope("thing we need to look at", "SCOPE.two", null),
            new Scope("are the names", "SCOPE.THREE", null)
        };
        
        var entity = GetEntity(scopes: initialScopes);
        entity.SetScopes(updatedScopes);

        entity.IsDirty.Should().BeFalse();
        entity.DateModified.Should().Be(DateTime.UnixEpoch);
        entity.Scopes.Should().BeEquivalentTo(initialScopes);
    }
    
    [Fact]
    public void SetScopes_ModifiesEntity_WhenScopesAdded()
    {
        var initialScopes = new[]
        {
            new Scope("grants can", "scope.one", null)
        };
        
        var updatedScopes = new[]
        {
            new Scope("so the only", "scope.ONE", null),
            new Scope("thing we need to look at", "SCOPE.two", null),
            new Scope("are the names", "SCOPE.THREE", null)
        };

        var expectedScopes = initialScopes.Concat(updatedScopes.Skip(1));
        
        var entity = GetEntity(scopes: initialScopes);
        entity.SetScopes(updatedScopes);

        entity.IsDirty.Should().BeTrue();
        entity.DateModified.Should().NotBe(DateTime.UnixEpoch);
        entity.Scopes.Should().BeEquivalentTo(expectedScopes);
    }
    
    [Fact]
    public void SetScopes_ModifiesEntity_WhenScopesRemoved()
    {
        var initialScopes = new[]
        {
            new Scope("grants can", "scope.one", null),
            new Scope("only reference", "scope.two", null),
            new Scope("one api", "scope.three", null)
        };
        
        var updatedScopes = new[]
        {
            new Scope("so the only", "scope.ONE", null)
        };

        var expectedScopes = initialScopes.Take(1);
        
        var entity = GetEntity(scopes: initialScopes);
        entity.SetScopes(updatedScopes);

        entity.IsDirty.Should().BeTrue();
        entity.DateModified.Should().NotBe(DateTime.UnixEpoch);
        entity.Scopes.Should().BeEquivalentTo(expectedScopes);
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
        int clientNumber = 1,
        int resourceServerNumber = 2,
        params Scope[]? scopes)
    {
        Grant entity;
        var client = GetClient(clientNumber);
        var resourceServer = GetResourceServer(resourceServerNumber);
        
        using (new DateTimeOffsetProviderContext(BaseDate))
        {
            entity = new Grant(externalId, client.Id, resourceServer.Id)
            {
                Scopes = (scopes ?? []).ToList(),
                DateModified = DateTime.UnixEpoch,
                Client = client,
                ResourceServer = resourceServer
            };
        }
        return entity;
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