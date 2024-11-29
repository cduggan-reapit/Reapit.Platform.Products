using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Api.Extensions;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Api.UnitTests.Extensions;

public class EntityCollectionExtensionsTests
{
    /*
     * GetMaximumCursor
     */

    [Fact]
    public void GetMaximumCursor_ReturnsZero_WhenProvidedAnEmptyCollection()
    {
        var collection = Array.Empty<ResourceServer>();
        var actual = collection.GetMaximumCursor();
        actual.Should().Be(0);
    }
    
    [Fact]
    public void GetMaximumCursor_ReturnsMaximumValue_WhenCollectionPopulated()
    {
        using var _ = new DateTimeOffsetProviderContext(DateTimeOffset.UnixEpoch.AddMicroseconds(500));
        
        // cursor should be X for unix epoch, +500 for the test
        const long expected = 500;

        var collection = new[] { new App("name") };
        var actual = collection.GetMaximumCursor();
        actual.Should().Be(expected);
    }
}