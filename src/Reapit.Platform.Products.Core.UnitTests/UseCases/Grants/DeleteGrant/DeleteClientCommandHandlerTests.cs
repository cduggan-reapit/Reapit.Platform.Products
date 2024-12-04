using Reapit.Platform.Products.Core.Services.IdentityProvider;
using Reapit.Platform.Products.Core.UseCases.Grants.DeleteGrant;
using Reapit.Platform.Products.Data.Repositories.Grants;
using Reapit.Platform.Products.Data.Services;
using Command = Reapit.Platform.Products.Core.UseCases.Grants.DeleteGrant.DeleteGrantCommand;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Grants.DeleteGrant;

public class DeleteGrantCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IGrantRepository _repository = Substitute.For<IGrantRepository>();
    private readonly IIdentityProviderService _idpService = Substitute.For<IIdentityProviderService>();
    private readonly FakeLogger<DeleteGrantCommandHandler> _logger = new();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsNotFoundException_WhenResourceServerNotFound()
    {
        _repository.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Entities.Grant?>(null));

        var command = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(command, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ReturnsTrue_WhenResourceServerDeleted()
    {
        var command = GetRequest();
        var entity = new Entities.Grant("external-id", "client-id", "resource-server-id")
        {
            Client = default!,
            ResourceServer = default!
        };

        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(entity);

        var sut = CreateSut();
        var actual = await sut.Handle(command, default);
        actual.DateDeleted.Should().NotBeNull();

        await _idpService.Received(1).DeleteGrantAsync(entity, Arg.Any<CancellationToken>());
        await _repository.Received(1).UpdateAsync(actual, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    /*
     * Private methods
     */
    
    private DeleteGrantCommandHandler CreateSut()
    {
        _unitOfWork.Grants.Returns(_repository);
        return new DeleteGrantCommandHandler(_unitOfWork, _idpService, _logger);
    }

    private static Command GetRequest(string id = "id")
        => new(id);
}