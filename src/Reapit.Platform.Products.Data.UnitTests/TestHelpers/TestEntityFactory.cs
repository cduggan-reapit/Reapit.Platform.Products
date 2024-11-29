using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Data.UnitTests.TestHelpers;

public static class TestEntityFactory
{
    public static App CreateApp(string name = "name", string? description = "description", bool skipConsent = false) 
        => new(name, description, skipConsent)
        {
            DateModified = DateTimeOffsetProvider.Now.UtcDateTime.AddYears(1)
        };
    
    public static Client CreateClient(
        string appId,
        string externalId = "external-id",
        ClientType? type = null,
        string name = "name", 
        string? description = "description", 
        string? loginUrl = null,
        ICollection<string>? callbackUrls = null,
        ICollection<string>? signOutUrls = null) 
        => new(appId, externalId, type ?? ClientType.Machine, name, description, loginUrl, callbackUrls, signOutUrls)
        {
            DateModified = DateTimeOffsetProvider.Now.UtcDateTime.AddYears(1)
        };
    
    public static ResourceServer CreateResourceServer(
        string externalId = "external-id", 
        string audience = "audience", 
        string name = "name", 
        int tokenLifetime = 3600,
        int scopes = 0)
    {
        var entity = new ResourceServer(externalId, audience, name, tokenLifetime)
        {
            DateModified = DateTimeOffsetProvider.Now.UtcDateTime.AddYears(1),
        };
        
        for(var i = 0; i < scopes; i++)
            entity.Scopes.Add(CreateScope(entity.Id, $"scope.{i:D3}"));

        return entity;
    }

    public static Scope CreateScope(string resourceServerId, string value = "example.scope", string? description = null) =>
        new(resourceServerId, value, description);
}