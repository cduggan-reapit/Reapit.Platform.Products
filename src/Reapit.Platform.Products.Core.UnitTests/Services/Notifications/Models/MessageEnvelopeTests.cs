using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Core.Services.Notifications.Models;

namespace Reapit.Platform.Products.Core.UnitTests.Services.Notifications.Models;

public class MessageEnvelopeTests
{
    /*
     * Ctor (and getters)
     */
    
    [Fact]
    public void Ctor_ShouldSetVersion()
    {
        var sut = new MessageEnvelope("entityType", "action", 2, new { Type = "Example" });
        sut.Version.Should().Be(2);
    }
    
    [Fact]
    public void Ctor_ShouldSetTimestamp_ToNow()
    {
        var time = new DateTimeOffset(1990, 1, 26, 10, 15, 47, TimeSpan.Zero);
        using var timeProvider = new DateTimeOffsetProviderContext(time);

        var sut = new MessageEnvelope("entityType", "action", 2, new { Type = "Example" });
        sut.Timestamp.Should().Be(time);
    }
    
    [Fact]
    public void Ctor_ShouldSetType_FromParts()
    {
        var sut = new MessageEnvelope("entityType", "action", 2, new { Type = "Example" });
        sut.Type.Should().Be("entityType.action");
    }
    
    [Fact]
    public void Ctor_ShouldSetPayloadString_AndHash()
    {
        var payloadObject = new { Type = "Example" };
        
        var sut = new MessageEnvelope("entityType", "action", 2, new { Type = "Example" });
        sut.Payload.Should().Be(payloadObject);
    }

    [Fact]
    public void ProductCreated_ShouldCreateMessageEnvelope_WithExpectedContent()
    {
        var resourceServer = new Entities.ResourceServer("not represented", "not represented", "product name one", 3600);

        using var _ = new DateTimeOffsetProviderContext(DateTime.UnixEpoch.AddDays(3));
        var expected = new MessageEnvelope("product", "created", 1, resourceServer.AsSerializable());
        var actual = MessageEnvelope.ProductCreated(resourceServer);
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void ProductModified_ShouldCreateMessageEnvelope_WithExpectedContent()
    {
        var resourceServer = new Entities.ResourceServer("not represented", "not represented", "product name two", 3600);

        using var _ = new DateTimeOffsetProviderContext(DateTime.UnixEpoch.AddDays(3));
        var expected = new MessageEnvelope("product", "modified", 1, resourceServer.AsSerializable());
        var actual = MessageEnvelope.ProductModified(resourceServer);
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void ProductDeleted_ShouldCreateMessageEnvelope_WithExpectedContent()
    {
        var resourceServer = new Entities.ResourceServer("not represented", "not represented", "product name three", 3600);

        using var _ = new DateTimeOffsetProviderContext(DateTime.UnixEpoch.AddDays(3));
        var expected = new MessageEnvelope("product", "deleted", 1, resourceServer.AsSerializable());
        var actual = MessageEnvelope.ProductDeleted(resourceServer);
        actual.Should().BeEquivalentTo(expected);
    }
}