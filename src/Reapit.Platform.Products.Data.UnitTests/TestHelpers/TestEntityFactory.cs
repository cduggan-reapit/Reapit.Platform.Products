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
}