using System.Text.Json;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Domain.UnitTests.Entities;

public class ProductClientTests
{
    /*
     * Ctor
     */

    [Fact]
    public void Ctor_InitializesProperties_FromParameters()
    {
        var fixedId = Guid.NewGuid();
        using var guidContext = new GuidProviderContext(fixedId);
        
        var fixedDate = new DateTimeOffset(2024, 11, 26, 15, 2, 32, TimeSpan.Zero);
        var expectedEpochTime = (long)(fixedDate - DateTimeOffset.UnixEpoch).TotalMicroseconds;
        using var timeContext = new DateTimeOffsetProviderContext(fixedDate);
        
        const string productId = "productId",
            clientId = "clientId",
            grantId = "grantId",
            name = "name",
            description = "description",
            audience = "audience";
        var type = ClientType.ClientCredentials;
        var callbackUrls = new[] { "callbackUrl" };
        var signOutUrls = new[] { "signOutUrl" };
        
        var entity = new ProductClient(productId, clientId, grantId, name, description, type, audience, callbackUrls, signOutUrls);
        
        // Explicit
        entity.ProductId.Should().Be(productId);
        entity.ClientId.Should().Be(clientId);
        entity.GrantId.Should().Be(grantId);
        entity.Name.Should().Be(name);
        entity.Description.Should().Be(description);
        entity.Type.Should().Be(type);
        entity.Audience.Should().Be(audience);
        entity.CallbackUrls.Should().BeEquivalentTo(callbackUrls);
        entity.SignOutUrls.Should().BeEquivalentTo(signOutUrls);
        
        // Implicit
        entity.Id.Should().Be($"{fixedId:N}");
        entity.DateCreated.Should().Be(fixedDate.UtcDateTime);
        entity.DateModified.Should().Be(fixedDate.UtcDateTime);
        entity.Cursor.Should().Be(expectedEpochTime);
    }
    
    /*
     * Update
     */

    [Fact]
    public void Update_UpdatesSimpleValue_WhenProvided()
    {
        const string name = "new name";
        var entity = GetProductClient();
        entity.Update(name: name);
        entity.IsDirty.Should().BeTrue();
        entity.Name.Should().Be(name);
    }
    
    [Fact]
    public void Update_UpdatesArray_WhenProvided()
    {
        var newValue = new[] { "differentCallbackUrl" };
        var entity = GetProductClient(callbackUrls: ["callbackUrl"]);
        entity.Update(callbackUrls: newValue);
        entity.IsDirty.Should().BeTrue();
        entity.CallbackUrls.Should().BeEquivalentTo(newValue);
    }
    
    [Fact]
    public void Update_DoesNotUpdate_WhenNoValuesProvided()
    {
        var entity = GetProductClient(callbackUrls: ["callbackUrl"], signOutUrls: ["signOutUrl"]);
        entity.Update();
        entity.IsDirty.Should().BeFalse();
    }
    
    [Fact]
    public void Update_DoesNotUpdate_WhenCurrentValuesProvided()
    {
        var entity = GetProductClient(callbackUrls: ["callbackUrl"], signOutUrls: ["signOutUrl"]);
        entity.Update(entity.Name, entity.Description, entity.CallbackUrls, entity.SignOutUrls);
        entity.IsDirty.Should().BeFalse();
    }
    
    /*
     * SoftDelete
     */
    
    [Fact]
    public void SoftDelete_SetsDateDeleted_WhenCalled()
    {
        using var timeFixture = new DateTimeOffsetProviderContext(DateTimeOffset.UnixEpoch);
        var entity = GetProductClient();
        entity.DateDeleted.Should().BeNull();
        
        var fixedDate = new DateTimeOffset(2024, 10, 18, 15, 12, 17, TimeSpan.FromHours(1));
        using var secondTimeFixture = new DateTimeOffsetProviderContext(fixedDate);

        entity.SoftDelete();

        entity.DateDeleted.Should().Be(fixedDate.UtcDateTime);
    }
    
    /*
     * AsSerializable
     */

    [Fact]
    public void AsSerializable_ReturnsAnonymousObject_ForGroup()
    {
        var entity = GetProductClient();
        var expected = new { entity.Id, entity.ProductId, entity.Name, entity.DateCreated, entity.DateModified };
        
        var actual = entity.AsSerializable();
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * ToString
     */

    [Fact]
    public void ToString_ReturnsSerializedObject_ForEntity()
    {
        var entity = GetProductClient();
        var expected = JsonSerializer.Serialize(entity.AsSerializable());
        var actual = entity.ToString();
        actual.Should().Be(expected);
    }
    
    /*
     * Private methods
     */

    private static ProductClient GetProductClient(string productId = "productId",
        string clientId = "clientId",
        string grantId = "grantId",
        string name = "name",
        string? description = "description",
        ClientType? type = null,
        string? audience = null,
        ICollection<string>? callbackUrls = null,
        ICollection<string>? signOutUrls = null)
        => new(productId, clientId, grantId, name, description, type ?? ClientType.ClientCredentials, audience, callbackUrls, signOutUrls);
}