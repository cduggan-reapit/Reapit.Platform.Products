using FluentValidation.Results;
using Reapit.Platform.Products.Core.Services.IdentityProvider;
using Reapit.Platform.Products.Core.UseCases.Grants.PatchGrant;
using Reapit.Platform.Products.Data.Repositories.Grants;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Grants.PatchGrant;

public class PatchGrantCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IGrantRepository _repository = Substitute.For<IGrantRepository>();
    private readonly IIdentityProviderService _idpService = Substitute.For<IIdentityProviderService>();
    private readonly IValidator<PatchGrantCommand> _validator = Substitute.For<IValidator<PatchGrantCommand>>();
    private readonly FakeLogger<PatchGrantCommandHandler> _logger = new();

    [Fact]
    public async Task Handle_ThrowsValidationException_WhenValidationFailed()
    {
        var request = GetRequest("arbitrary");
        SetupValidator(false);
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_ThrowsNotFoundException_WhenEntityDoesNotExist()
    {
        var request = GetRequest("arbitrary");
        SetupValidator(true);
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ReturnsEntity_WhenNoChangesApplied()
    {
        var request = GetRequest("arbitrary", "scope.one", "scope.two");
        
        SetupValidator(true);

        var resourceServer = GetResourceServer("scope.zero", "scope.one", "scope.two", "scope.three");
        var grant = GetGrant(resourceServer, "scope.one", "scope.two");
        _repository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(grant);
        
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeEquivalentTo(grant);

        await _repository.DidNotReceiveWithAnyArgs().UpdateAsync(grant, default);
        await _idpService.DidNotReceiveWithAnyArgs().UpdateGrantAsync(grant, default);
    }
    
    [Fact]
    public async Task Handle_ReturnsEntity_WhenChangesApplied()
    {
        var request = GetRequest("arbitrary", "scope.one", "scope.two", "scope.three");
        
        SetupValidator(true);

        var resourceServer = GetResourceServer("scope.zero", "scope.one", "scope.two", "scope.three");
        var grant = GetGrant(resourceServer, "scope.one", "scope.two");
        _repository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(grant);
        
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeEquivalentTo(grant);

        await _repository.Received(1).UpdateAsync(grant, Arg.Any<CancellationToken>());
        await _idpService.Received(1).UpdateGrantAsync(grant, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private PatchGrantCommandHandler CreateSut()
    {
        _unitOfWork.Grants.Returns(_repository);
        return new PatchGrantCommandHandler(_unitOfWork, _idpService, _validator, _logger);
    }

    private static PatchGrantCommand GetRequest(string id, params string[]? scopes)
        => new(id, scopes ?? []);
    
    private void SetupValidator(bool isValid)
        => _validator.ValidateAsync(Arg.Any<PatchGrantCommand>(), Arg.Any<CancellationToken>())
            .Returns(isValid
                ? new ValidationResult()
                : new ValidationResult([new ValidationFailure("propertyName", "errorMessage")]));

    private static Entities.Grant GetGrant(Entities.ResourceServer resourceServer, params string[]? scopes)
        => new("", "", "")
        {
            ResourceServer = resourceServer,
            Scopes = (scopes ?? []).Select(scope => new Entities.Scope("", scope, null)).ToList()
        };
    
    private static Entities.ResourceServer GetResourceServer(params string[]? scopes)
        => new("", "", "", 1)
        {
            Scopes = (scopes ?? []).Select(scope => new Entities.Scope("", scope, "")).ToList()
        };
}