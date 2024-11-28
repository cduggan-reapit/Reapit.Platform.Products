using Reapit.Platform.Products.Core.Extensions;

namespace Reapit.Platform.Products.Core.UnitTests.Extensions;

public class ObjectExtensionTests
{
    /*
     * ToJson
     */

    [Fact]
    public void ToJson_SerializesObject_WithDefaultSettings()
    {
        var input = new { Property = "Value" };
        const string expected = "{\"property\":\"Value\"}";
        var actual = input.ToJson();
        actual.Should().Be(expected);
    }
    
    [Fact]
    public void ToJson_SerializesEmptyObject_WhenNullProvided()
    {
        const string expected = "{}";
        var actual = (null as object).ToJson();
        actual.Should().Be(expected);
    }
}