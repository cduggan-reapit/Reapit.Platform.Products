using System.Text.Json;
using System.Text.Json.Serialization;
using Reapit.Platform.Products.Core.Extensions;
using Xunit.Abstractions;

namespace Reapit.Platform.Products.Core.UnitTests.Extensions;

public class StringExtensions
{
    [Fact]
    public void DeserializeTo_DeserializesString_UsingDefaultSettings()
    {
        // Default is case-insensitive so we test with property
        const string value = "value";
        const string input = $"{{ \"Property\": \"{value}\" }}";
        var actual = input.DeserializeTo<TestObject>();
        actual.Property.Should().Be(value);
    }
    
    [Fact]
    public void DeserializeTo_DeserializesString_UsingConfiguredSettings()
    {
        const string value = "value";
        const string input = $"{{ \"property\": \"{value}\" }}";
        var actual = input.DeserializeTo<TestObject>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
        });
        actual.Property.Should().Be(value);
    }
    
    [Fact]
    public void DeserializeTo_ThrowsException_WhenRequiredPropertyMissing()
    {
        const string input = "{ }";
        var action = () => input.DeserializeTo<TestObject>();
        action.Should().Throw<JsonException>();
    }
    
    [Fact]
    public void DeserializeTo_ThrowsException_WhenInvalidJson()
    {
        const string input = "{ invalid: json }";
        var action = () => input.DeserializeTo<TestObject>();
        action.Should().Throw<JsonException>();
    }

    private record struct TestObject([property: JsonPropertyName("Property"), JsonRequired] string Property);
}