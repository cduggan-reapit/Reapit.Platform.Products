using FluentValidation.Results;
using Reapit.Platform.Products.Core.UseCases.Applications.CreateApplication;
using Reapit.Platform.Products.Data.Repositories.Apps;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;
using Command = Reapit.Platform.Products.Core.UseCases.Applications.CreateApplication.CreateApplicationCommand;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Applications.CreateApplication;

public class CreateApplicationCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IAppRepository _repository = Substitute.For<IAppRepository>();
    private readonly IValidator<CreateApplicationCommand> _validator = Substitute.For<IValidator<CreateApplicationCommand>>();
    private readonly FakeLogger<CreateApplicationCommandHandler> _logger = new();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsValidationException_WhenValidationFailed()
    {
        SetupValidator(false);
        var command = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(command, default);

        await action.Should().ThrowAsync<ValidationException>();
        await _repository.DidNotReceive().CreateAsync(Arg.Any<App>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CreatesResourceServer_WhenRequestHandledSuccessfully()
    {
        const string externalId = "auth0-identifier";
        var command = GetRequest();
        
        SetupValidator(true);
        
        var sut = CreateSut();
        var actual = await sut.Handle(command, default);
        
        // Check the created resource server properties
        actual.Name.Should().Be(command.Name);
        actual.Description.Should().Be(command.Description);
        actual.IsFirstParty.Should().Be(command.IsFirstParty);
        
        // And the database calls
        await _repository.Received(1).CreateAsync(actual, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private CreateApplicationCommandHandler CreateSut()
    {
        _unitOfWork.Apps.Returns(_repository);
        return new CreateApplicationCommandHandler(_unitOfWork, _validator, _logger);
    }

    private void SetupValidator(bool isSuccess) 
        => _validator.ValidateAsync(Arg.Any<CreateApplicationCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(isSuccess 
                ? []
                : new[] { new ValidationFailure("propertyName", "errorMessage") }));
    
    private static Command GetRequest(
        string name = "name",
        string description = "description",
        bool isFirstParty = false)
        => new(name, description, isFirstParty);
}