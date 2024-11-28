using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Domain.UnitTests.Entities.Enums;

public class ClientTypeTests
{
    /*public static implicit operator ClientType(string name) 
        => GetByName(name) ?? throw new ArgumentException($"Invalid client name: {name}");
    
    public static implicit operator ClientType(int value) 
        => GetByValue(value) ?? throw new ArgumentException($"Invalid client type value: {value}");
    
    public static implicit operator string(ClientType clientType) => clientType.Name;
    public static implicit operator int(ClientType clientType) => clientType.Value;*/
    
    /*
     * Implicit Operators
     */

    [Fact]
    public void ClientType_ReturnsName_WhenCastToString()
    {
        var clientType = ClientType.ClientCredentials;
        string actual = clientType;
        actual.Should().Be(clientType.Name);
    }
    
    [Fact]
    public void ClientType_ReturnsValue_WhenCastToInt()
    {
        var clientType = ClientType.ClientCredentials;
        int actual = clientType;
        actual.Should().Be(clientType.Value);
    }
    
    [Fact]
    public void ClientType_ReturnsClientType_WhenCastFromString()
    {
        var clientType = ClientType.AuthorizationCode;
        ClientType? actual = clientType.Name;
        actual.Should().Be(clientType);
    }
    
    [Fact]
    public void ClientType_ReturnsClientType_WhenCastFromInt()
    {
        var clientType = ClientType.AuthorizationCode;
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
}