using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Domain.UnitTests.Entities;

public class ScopeTests
{
    /*
     * Ctor
     */

    [Fact]
    public void Ctor_SetsProperties_FromParameters()
    {
        const string value = "value", description = "description", resourceServerId = "resourceServerId";
        var entity = new Scope(resourceServerId, value, description);

        entity.Value.Should().Be(value);
        entity.Description.Should().Be(description);
        entity.ResourceServerId.Should().Be(resourceServerId);
    }
}