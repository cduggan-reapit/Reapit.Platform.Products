using Reapit.Platform.Products.Core.Services.IdentityProvider;
using Reapit.Platform.Products.Core.UseCases.Clients.CreateClient;
using Reapit.Platform.Products.Core.UseCases.Grants.CreateGrant;
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

    public Task<string> CreateAuthCodeClientAsync(CreateClientCommand command, bool isFirstParty, CancellationToken cancellationToken)
        => Task.FromResult("external-id-for" + command.Name);

    public Task<string> CreateMachineClientAsync(CreateClientCommand command, bool isFirstParty, CancellationToken cancellationToken)
        => Task.FromResult("external-id-for" + command.Name);

    public Task<bool> UpdateAuthCodeClientAsync(Client entity, CancellationToken cancellationToken)
        => Task.FromResult(true);

    public Task<bool> UpdateMachineClientAsync(Client entity, CancellationToken cancellationToken)
        => Task.FromResult(true);

    public Task<bool> DeleteClientAsync(Client entity, CancellationToken cancellationToken)
        => Task.FromResult(true);

    public Task<string> CreateGrantAsync(CreateGrantCommand command, Client client, ResourceServer resourceServer, CancellationToken cancellationToken)
        => Task.FromResult("external-id");

    public Task<bool> UpdateGrantAsync(Grant grant, CancellationToken cancellationToken)
        => Task.FromResult(true);

    public Task<bool> DeleteGrantAsync(Grant grant, CancellationToken cancellationToken)
        => Task.FromResult(true);
}