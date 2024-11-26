using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Core.Services.Notifications.Models;
using Reapit.Platform.Products.Domain.Entities;

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
    
    /*
     * ProductCreated
     */

    [Fact]
    public void ProductCreated_CreatesInstance_FromProductObject()
    {
        var time = DateTimeOffset.UnixEpoch;
        using var timeProvider = new DateTimeOffsetProviderContext(time);
        
        var product = new Product("name", "description", "reference");
        var expected = new MessageEnvelope("product", "created", 1, product.AsSerializable());
        
        var actual = MessageEnvelope.ProductCreated(product);
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * ProductModified
     */

    [Fact]
    public void ProductModified_CreatesInstance_FromProductObject()
    {
        var time = DateTimeOffset.UnixEpoch;
        using var timeProvider = new DateTimeOffsetProviderContext(time);
        
        var product = new Product("name", "description", "reference");
        var expected = new MessageEnvelope("product", "modified", 1, product.AsSerializable());
        
        var actual = MessageEnvelope.ProductModified(product);
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * ProductDeleted
     */

    [Fact]
    public void ProductDeleted_CreatesInstance_FromProductObject()
    {
        var time = DateTimeOffset.UnixEpoch;
        using var timeProvider = new DateTimeOffsetProviderContext(time);
        
        var product = new Product("name", "description", "reference");
        var expected = new MessageEnvelope("product", "deleted", 1, product.AsSerializable());
        
        var actual = MessageEnvelope.ProductDeleted(product);
        actual.Should().BeEquivalentTo(expected);
    }
}