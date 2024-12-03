using Reapit.Platform.Common.Enums;

namespace Reapit.Platform.Products.Domain.Entities.Enums;

public class ClientType(string name, int value)
    : SmartEnum<ClientType, int>(name, value)
{
    public static readonly ClientType Machine = new("machine", 1);
    
    public static readonly ClientType AuthCode = new("authCode", 2);
    
    public static implicit operator ClientType?(string name) => GetByName(name);
    
    public static implicit operator ClientType?(int value) => GetByValue(value);
    
    public static implicit operator string(ClientType clientType) => clientType.Name;
    public static implicit operator int(ClientType clientType) => clientType.Value;
};