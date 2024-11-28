using Reapit.Platform.Common.Enums;

namespace Reapit.Platform.Products.Domain.Entities.Enums;

public class ClientType(string name, int id) : SmartEnum<ClientType, int>(name, id)
{
    public static readonly ClientType ClientCredentials = new("client_credentials", 1);
    
    public static readonly ClientType AuthorizationCode = new("authorization_code", 2);
    
    public static implicit operator ClientType(string name) 
        => GetByName(name) ?? throw new ArgumentException($"Invalid client name: {name}");
    
    public static implicit operator ClientType(int value) 
        => GetByValue(value) ?? throw new ArgumentException($"Invalid client type value: {value}");
    
    public static implicit operator string(ClientType clientType) => clientType.Name;
    public static implicit operator int(ClientType clientType) => clientType.Value;
}