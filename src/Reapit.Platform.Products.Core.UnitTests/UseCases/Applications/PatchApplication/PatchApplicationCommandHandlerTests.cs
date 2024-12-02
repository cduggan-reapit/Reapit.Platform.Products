using FluentValidation.Results;
using Reapit.Platform.Products.Core.UseCases.Applications.PatchApplication;
using Reapit.Platform.Products.Data.Repositories.Apps;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Applications.PatchApplication;

public class PatchApplicationCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IAppRepository _repository = Substitute.For<IAppRepository>();
    private readonly IValidator<PatchApplicationCommand> _validator = Substitute.For<IValidator<PatchApplicationCommand>>();
    private readonly FakeLogger<PatchApplicationCommandHandler> _logger = new();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsValidationException_WhenValidationFailed()
    {
        SetupValidator(false);
        var request = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_ThrowsNotFoundException_WhenEntityNotFound()
    {
        var request = GetRequest();
        SetupValidator(true);

        _repository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<App?>(null));
        
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_DoesNotApplyUpdate_WhenEntityUnchanged()
    {
        var entity = new App("real friends", "description of the app", false);
        var request = GetRequest(entity.Id, entity.Name, entity.Description);
        
        SetupValidator(true);

        _repository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(entity);

        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeEquivalentTo(entity);

        await _repository.DidNotReceive().UpdateAsync(Arg.Any<App>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AppliesUpdate_WhenEntityChanged()
    {
        var entity = new App("catch 22", "description of the app", false);
        var request = GetRequest(entity.Id, "streetlight manifesto", entity.Description);
        
        SetupValidator(true);

        _repository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(entity);

        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeEquivalentTo(entity);

        await _repository.Received(1).UpdateAsync(Arg.Any<App>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private PatchApplicationCommandHandler CreateSut()
    {
        _unitOfWork.Apps.Returns(_repository);
        return new PatchApplicationCommandHandler(_unitOfWork, _validator, _logger);
    }
    
    private void SetupValidator(bool isSuccess) 
        => _validator.ValidateAsync(Arg.Any<PatchApplicationCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(isSuccess 
                ? []
                : new[] { new ValidationFailure("propertyName", "errorMessage") }));

    private static PatchApplicationCommand GetRequest(string id = "id", string? name = null, string? description = null)
        => new(id, name, description);
}