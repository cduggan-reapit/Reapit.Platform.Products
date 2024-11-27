using Reapit.Platform.Common.Enums;

namespace Reapit.Platform.Products.Domain.Entities.Enums;

public class ClientType(string name, int id) : SmartEnum<ClientType, int>(name, id)
{
    public static readonly ClientType ClientCredentials = new("client_credentials", 1);
    
    public static readonly ClientType AuthorizationCode = new("authorization_code", 2);
}