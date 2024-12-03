using Reapit.Platform.Products.Core.Services.IdentityProvider;
using Reapit.Platform.Products.Core.UseCases.Clients.DeleteClient;
using Reapit.Platform.Products.Data.Repositories.Clients;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities.Enums;
using Command = Reapit.Platform.Products.Core.UseCases.Clients.DeleteClient.DeleteClientCommand;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Clients.DeleteClient;

public class DeleteClientCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IClientRepository _repository = Substitute.For<IClientRepository>();
    private readonly IIdentityProviderService _idpService = Substitute.For<IIdentityProviderService>();
    private readonly FakeLogger<DeleteClientCommandHandler> _logger = new();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsNotFoundException_WhenResourceServerNotFound()
    {
        _repository.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Entities.Client?>(null));

        var command = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(command, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ReturnsTrue_WhenResourceServerDeleted()
    {
        var command = GetRequest();
        var entity = new Entities.Client("", "", ClientType.Machine, "", null, null, null, null);

        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(entity);

        var sut = CreateSut();
        var actual = await sut.Handle(command, default);
        actual.DateDeleted.Should().NotBeNull();

        await _idpService.Received(1).DeleteClientAsync(entity, Arg.Any<CancellationToken>());
        await _repository.Received(1).UpdateAsync(actual, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    /*
     * Private methods
     */
    
    private DeleteClientCommandHandler CreateSut()
    {
        _unitOfWork.Clients.Returns(_repository);
        return new DeleteClientCommandHandler(_unitOfWork, _idpService, _logger);
    }

    private static Command GetRequest(string id = "id")
        => new(id);
}