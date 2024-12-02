using Reapit.Platform.Products.Core.UseCases.Applications;
using Reapit.Platform.Products.Core.UseCases.Applications.DeleteApplication;
using Reapit.Platform.Products.Data.Repositories.Apps;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;
using Command = Reapit.Platform.Products.Core.UseCases.Applications.DeleteApplication.DeleteApplicationCommand;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Applications.DeleteApplication;

public class DeleteApplicationCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IAppRepository _repository = Substitute.For<IAppRepository>();
    private readonly FakeLogger<DeleteApplicationCommandHandler> _logger = new();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsNotFoundException_WhenAppNotFound()
    {
        _repository.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<App?>(null));

        var command = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(command, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task Handle_ThrowsValidationException_WhenAppHasClients()
    {
        var command = GetRequest();
        var entity = new App("name", "description", true)
        {
            Clients = { new Client("appId", "externalId", ClientType.Machine, "name", "description", null, null, null) }
        };

        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(entity);

        var sut = CreateSut();
        var action = () => sut.Handle(command, default);
        await action.Should().ThrowAsync<ValidationException>()
            .WithMessage($"*{ApplicationValidationMessages.ClientsPreventingDelete}*");

        await _repository.DidNotReceive().UpdateAsync(Arg.Any<App>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ReturnsTrue_WhenAppDeleted()
    {
        var command = GetRequest();
        var entity = new App("name", "description", true);

        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(entity);

        var sut = CreateSut();
        var actual = await sut.Handle(command, default);
        actual.DateDeleted.Should().NotBeNull();

        await _repository.Received(1).UpdateAsync(actual, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    /*
     * Private methods
     */
    
    private DeleteApplicationCommandHandler CreateSut()
    {
        _unitOfWork.Apps.Returns(_repository);
        return new DeleteApplicationCommandHandler(_unitOfWork, _logger);
    }

    private static Command GetRequest(string id = "id")
        => new(id);
}