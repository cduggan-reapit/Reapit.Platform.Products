using FluentValidation.Results;
using Reapit.Platform.Products.Core.Services.IdentityProvider;
using Reapit.Platform.Products.Core.UseCases.Common.Scopes;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.CreateResourceServer;
using Reapit.Platform.Products.Data.Repositories.ResourceServers;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.ResourceServers.CreateResourceServer;

public class CreateResourceServerCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IResourceServerRepository _repository = Substitute.For<IResourceServerRepository>();
    private readonly IIdentityProviderService _idpService = Substitute.For<IIdentityProviderService>();
    private readonly IValidator<CreateResourceServerCommand> _validator = Substitute.For<IValidator<CreateResourceServerCommand>>();
    private readonly FakeLogger<CreateResourceServerCommandHandler> _logger = new();
    
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
        await _repository.DidNotReceive().CreateAsync(Arg.Any<ResourceServer>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CreatesResourceServer_WhenRequestHandledSuccessfully()
    {
        const string externalId = "auth0-identifier";
        var command = GetRequest(scopes: [
            new RequestScopeModel("scope.one", "scope.one description"),
            new RequestScopeModel("scope.two", "scope.two description"),
            new RequestScopeModel("scope.three", "scope.three description")
        ]);
        
        SetupValidator(true);

        _idpService.CreateResourceServerAsync(command, Arg.Any<CancellationToken>())
            .Returns(externalId);

        var sut = CreateSut();
        var actual = await sut.Handle(command, default);
        
        // Check the created resource server properties
        actual.ExternalId.Should().Be(externalId);
        actual.Name.Should().Be(command.Name);
        actual.Audience.Should().Be(command.Audience);
        actual.TokenLifetime.Should().Be(command.TokenLifetime);
        actual.Scopes.Should().AllSatisfy(scope 
            => command.Scopes.Should().Contain(c => 
                c.Value == scope.Value && c.Description == scope.Description));
        
        // And the database calls
        await _repository.Received(1).CreateAsync(actual, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private CreateResourceServerCommandHandler CreateSut()
    {
        _unitOfWork.ResourceServers.Returns(_repository);
        return new CreateResourceServerCommandHandler(_unitOfWork, _idpService, _validator, _logger);
    }

    private void SetupValidator(bool isSuccess) 
        => _validator.ValidateAsync(Arg.Any<CreateResourceServerCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(isSuccess 
                ? []
                : new[] { new ValidationFailure("propertyName", "errorMessage") }));

    private static CreateResourceServerCommand GetRequest(
        string name = "name",
        string audience = "audience",
        int tokenLifetime = 3600,
        ICollection<RequestScopeModel>? scopes = null)
        => new(name, audience, tokenLifetime, scopes ?? []);
}