using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Domain.UnitTests.Entities.Enums;

public class ClientTypeTests
{ 
    /*
     * Implicit Operators
     */

    [Fact]
    public void ClientType_ReturnsName_WhenCastToString()
    {
        var clientType = ClientType.Machine;
        string actual = clientType;
        actual.Should().Be(clientType.Name);
    }
    
    [Fact]
    public void ClientType_ReturnsValue_WhenCastToInt()
    {
        var clientType = ClientType.Machine;
        int actual = clientType;
        actual.Should().Be(clientType.Value);
    }
    
    [Fact]
    public void ClientType_ReturnsClientType_WhenCastFromString()
    {
        var clientType = ClientType.AuthCode;
        ClientType? actual = clientType.Name;
        actual.Should().Be(clientType);
    }
    
    [Fact]
    public void ClientType_ReturnsClientType_WhenCastFromInt()
    {
        var clientType = ClientType.AuthCode;
        ClientType? actual = clientType.Value;
        actual.Should().Be(clientType);
    }
    
    [Fact]
    public void ClientType_ReturnsNull_WhenCastFromString_WithoutMatchingEnum()
    {
        ClientType? actual = "none";
        actual.Should().BeNull();
    }
    
    [Fact]
    public void ClientType_ReturnsNull_WhenCastFromInt_WithoutMatchingEnum()
    {
        ClientType? actual = -3;
        actual.Should().BeNull();
    }
    
    /*
     * GrantTypes
     */

    [Fact]
    public void ClientType_PopulatesGrantTypes_ForImplementations()
    {
        var expected = new[] { "authorization_code", "refresh_token" };
        var actual = ClientType.AuthCode.GrantTypes;
        actual.Should().BeEquivalentTo(expected);
    }
}