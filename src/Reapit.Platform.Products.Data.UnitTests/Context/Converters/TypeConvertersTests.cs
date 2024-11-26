using Reapit.Platform.Products.Data.Context.Converters;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Data.UnitTests.Context.Converters;

public class TypeConvertersTests
{
    /*
     * DateTimeConverter
     */
    
    [Fact]
    public void DateTimeConverter_ConvertsValueToPersist_ToUTC()
    {
        // We can't really check the value here, since Local could be... anything, but we can check that the value is
        // marked as a UTC date time.
        var valueToPersist = new DateTime(2020, 1, 1, 0 ,0, 0, DateTimeKind.Local);
        var sut = TypeConverters.DateTimeConverter;
        
        var actual = sut.ConvertToProviderTyped(valueToPersist);
        actual.Kind.Should().Be(DateTimeKind.Utc);
    }
    
    [Fact]
    public void DateTimeConverter_ConvertsValueToRetrieve_ToUTC()
    {
        // Database values are always unspecified, so check that it gets flagged as UTC on retrieval 
        var valueToRetrieve = new DateTime(2020, 1, 1, 0 ,0, 0, DateTimeKind.Unspecified);
        var sut = TypeConverters.DateTimeConverter;
        
        var actual = sut.ConvertFromProviderTyped(valueToRetrieve);
        actual.Kind.Should().Be(DateTimeKind.Utc);
    }
    
    /*
     * ClientTypeConverter
     */

    [Fact]
    public void ClientTypeConverter_ConvertsClientType_ToInteger()
    {
        var clientType = ClientType.ClientCredentials;
        var expected = clientType.Value;
        
        var sut = TypeConverters.ClientTypeConverter;
        var actual = sut.ConvertToProviderTyped(clientType);
        actual.Should().Be(expected);
    }
    
    [Fact]
    public void ClientTypeConverter_ConvertsInteger_ToClientType()
    {
        var expected = ClientType.AuthorizationCode;
        var integer = expected.Value;
        
        var sut = TypeConverters.ClientTypeConverter;
        var actual = sut.ConvertFromProviderTyped(integer);
        actual.Should().Be(expected);
    }
    
    /*
     * StringArrayConverter
     */

    [Fact]
    public void StringArrayConverter_ConvertsNull_ToNull()
    {
        // Test it both ways, no need to double-up on tests
        var sut = TypeConverters.StringArrayConverter;
        sut.ConvertToProviderTyped(null).Should().BeNull();
        sut.ConvertFromProviderTyped(null).Should().BeNull();
    }

    [Fact]
    public void StringArrayConverter_ConvertsCollection_ToString()
    {
        const char separator = ',';
        var input = new[] { "item one", "item two", "item three" };
        var expected = string.Join(separator, input);

        var sut = TypeConverters.StringArrayConverter;
        var actual = sut.ConvertToProviderTyped(input);
        actual.Should().Be(expected);
    }
    
    [Fact]
    public void StringArrayConverter_ConvertsString_ToCollection()
    {
        const char separator = ',';
        var expected = new[] { "item one", "item two", "item three" };
        var input = string.Join(separator, expected);

        var sut = TypeConverters.StringArrayConverter;
        var actual = sut.ConvertFromProviderTyped(input);
        actual.Should().BeEquivalentTo(expected);
    }
}