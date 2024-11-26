using Reapit.Platform.Common.Enums;

namespace Reapit.Platform.Products.Domain.Entities.Enums;

public class ClientType(string name, int id, params string[]? grantTypes) : SmartEnum<ClientType, int>(name, id)
{
    public IEnumerable<string> GrantTypes { get; } = grantTypes ?? [];
    
    public static readonly ClientType None = new("none", 0);
    
    public static readonly ClientType ClientCredentials = new("non_interactive", 1, "client_credentials");
    
    public static readonly ClientType AuthorizationCode = new("regular_web", 2, "authorization_code", "refresh_token");
}