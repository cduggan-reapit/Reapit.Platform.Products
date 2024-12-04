using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Core.Services.IdentityProvider;
using Reapit.Platform.Products.Core.Services.Notifications.Models;
using Reapit.Platform.Products.Core.UnitTests.TestServices;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.DeleteResourceServer;
using Reapit.Platform.Products.Data.Repositories.ResourceServers;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;
using Command = Reapit.Platform.Products.Core.UseCases.ResourceServers.DeleteResourceServer.DeleteResourceServerCommand;
using NotFoundException = Reapit.Platform.Common.Exceptions.NotFoundException;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.ResourceServers.DeleteResourceServer;

public class DeleteResourceServerCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IResourceServerRepository _repository = Substitute.For<IResourceServerRepository>();
    private readonly IIdentityProviderService _idpService = Substitute.For<IIdentityProviderService>();
    private readonly MockNotificationsService _notifications = new();
    private readonly FakeLogger<DeleteResourceServerCommandHandler> _logger = new();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsNotFoundException_WhenResourceServerNotFound()
    {
        _repository.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<ResourceServer?>(null));

        var command = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(command, default);
        await action.Should().ThrowAsync<NotFoundException>();
        
        _notifications.LastMessage.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ReturnsTrue_WhenResourceServerDeleted()
    {
        var command = GetRequest();
        var entity = new ResourceServer("external-id", "audience", "name", 3600);

        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(entity);

        using var _ = new DateTimeOffsetProviderContext(DateTimeOffset.UtcNow);
        var sut = CreateSut();
        var actual = await sut.Handle(command, default);
        actual.DateDeleted.Should().NotBeNull();

        _notifications.LastMessage.Should().NotBeNull()
            .And.BeEquivalentTo(MessageEnvelope.ProductDeleted(actual));
        await _idpService.Received(1).DeleteResourceServerAsync(entity, Arg.Any<CancellationToken>());
        await _repository.Received(1).UpdateAsync(actual, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    /*
     * Private methods
     */
    
    private DeleteResourceServerCommandHandler CreateSut()
    {
        _unitOfWork.ResourceServers.Returns(_repository);
        return new DeleteResourceServerCommandHandler(_unitOfWork, _idpService, _notifications, _logger);
    }

    private static Command GetRequest(string id = "id")
        => new(id);
}