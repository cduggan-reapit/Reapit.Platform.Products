using System.Text.Json;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Domain.UnitTests.Entities;

public class ClientTests
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
        
        const string appId = "app-id", 
            externalId = "external-id", 
            name = "name", 
            description = "description",
            loginUrl = "login-url";
        
        var type = ClientType.AuthCode;
        var callbackUrls = new[] { "callback-url" };
        var signOutUrls = new[] { "sign-out-url" };

        var entity = new Client(
            appId,
            externalId,
            type,
            name,
            description,
            loginUrl,
            callbackUrls,
            signOutUrls);
        
        // Explicit
        entity.AppId.Should().Be(appId);
        entity.ExternalId.Should().Be(externalId);
        entity.Name.Should().Be(name);
        entity.Description.Should().Be(description);
        entity.LoginUrl.Should().Be(loginUrl);
        entity.Type.Should().Be(type);
        entity.CallbackUrls.Should().BeEquivalentTo(callbackUrls);
        entity.SignOutUrls.Should().BeEquivalentTo(signOutUrls);
        
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
        var entity = GetEntity(callbackUrls: ["callback"], signOutUrls: ["sign-out"]);
        entity.Update(entity.Name, entity.Description, entity.LoginUrl, entity.CallbackUrls, entity.SignOutUrls);
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
    
    [Fact]
    public void Update_ModifiesEntity_WhenCollectionsReplaced()
    {
        const string callback = "new-callback";
        var entity = GetEntity(callbackUrls: ["originalCallback"]);
        entity.Update(callbackUrls: [callback]);
        entity.IsDirty.Should().BeTrue();
        entity.CallbackUrls.Should().HaveCount(1).And.AllSatisfy(c => c.Should().Be(callback));
        entity.DateModified.Should().NotBe(BaseDate.UtcDateTime);
    }
    
    /*
     * SoftDelete
     */

    [Fact]
    public void SoftDelete_SetsDateDeleted_WhenCalled()
    {
        var entity = GetEntity();
        entity.Grants.Add(new Grant("test", entity.Id, "resourceServerId"){ ResourceServer = default!, Client = default! });
        
        entity.SoftDelete();
        entity.DateDeleted.Should().NotBeNull();
        
        entity.Grants.Should().AllSatisfy(grant => grant.DateDeleted.Should().NotBeNull());
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
        var expected = JsonSerializer.Serialize(new { entity.Id, entity.Name, Type = entity.Type.Name, entity.DateCreated, entity.DateModified });
        var actual = entity.ToString();
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * Private methods
     */
    
    private static readonly DateTimeOffset BaseDate = new(2024, 11, 29, 13, 18, 17, TimeSpan.Zero);

    private static Client GetEntity(
        string appId = "app-id",
        string externalId = "external-id",
        ClientType? type = null,
        string name = "name", 
        string? description = "description", 
        string? loginUrl = "login-url",
        string[]? callbackUrls = null,
        string[]? signOutUrls = null)
    {
        Client entity;
        using (new DateTimeOffsetProviderContext(BaseDate))
        {
            entity = new Client(
                appId, 
                externalId, 
                type ?? ClientType.Machine, 
                name, 
                description, 
                loginUrl,
                callbackUrls,
                signOutUrls);
        }
        return entity;
    }
}