using Reapit.Platform.Products.Core.Services.IdentityProvider;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.CreateResourceServer;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Api.IntegrationTests.TestServices;

public class MockIdentityProviderService : IIdentityProviderService
{
    public Task<string> CreateResourceServerAsync(CreateResourceServerCommand command, CancellationToken cancellationToken)
        => Task.FromResult("external-id-for-" + command.Name);

    public Task<bool> UpdateResourceServerAsync(ResourceServer entity, CancellationToken cancellationToken)
        => Task.FromResult(true);

    public Task<bool> DeleteResourceServerAsync(ResourceServer entity, CancellationToken cancellationToken)
        => Task.FromResult(true);
}