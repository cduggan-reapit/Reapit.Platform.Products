using FluentValidation.Results;
using Reapit.Platform.Products.Core.Services.IdentityProvider;
using Reapit.Platform.Products.Core.UseCases.Common.Scopes;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.PatchResourceServer;
using Reapit.Platform.Products.Data.Repositories.ResourceServers;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.ResourceServers.PatchResourceServer;

public class PatchResourceServerCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IResourceServerRepository _repository = Substitute.For<IResourceServerRepository>();
    private readonly IIdentityProviderService _idpService = Substitute.For<IIdentityProviderService>();
    private readonly IValidator<PatchResourceServerCommand> _validator = Substitute.For<IValidator<PatchResourceServerCommand>>();
    private readonly FakeLogger<PatchResourceServerCommandHandler> _logger = new();
    
    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsValidationException_WhenResourceServerNotFound()
    {
        SetupValidator(false);
        var command = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(command, default);
        await action.Should().ThrowAsync<ValidationException>();
    }
    
    [Fact]
    public async Task Handle_ThrowsNotFoundException_WhenResourceServerNotFound()
    {
        SetupValidator(true);
        
        _repository.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<ResourceServer?>(null));
        
        var command = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(command, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ReturnsTrue_WhenResourceServerUnchanged()
    {
        SetupValidator(true);
        
        var scopeModels = new List<RequestScopeModel>
        {
            new("value.one", "description.one"),
            new("value.two", "description.two"),
            new("value.three", "description.three")
        };
        
        var entity = new ResourceServer("external-id", "audience", "name", 3600)
        {
            Id = "id",
            Scopes = scopeModels.Select(scope => new Scope("id", scope.Value, scope.Description)).ToList()
        };
        
        var command = GetRequest(entity.Id, entity.Name, entity.TokenLifetime, scopeModels);

        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(entity);

        var sut = CreateSut();
        var actual = await sut.Handle(command, default);
        actual.Should().Be(entity);

        await _idpService.DidNotReceiveWithAnyArgs().UpdateResourceServerAsync(Arg.Any<ResourceServer>(), Arg.Any<CancellationToken>());
        await _repository.DidNotReceiveWithAnyArgs().UpdateAsync(Arg.Any<ResourceServer>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Handle_ReturnsTrue_WhenResourceServerUpdated()
    {
        SetupValidator(true);
        
        var scopeModels = new List<RequestScopeModel>
        {
            new("value.one", "description.one"),
            new("value.two", "description.two"),
            new("value.three", "description.three")
        };
        
        // We'll say there were only two on the original entity. All of them should be replaced if we change the casing
        // of the description (although values will be case-insensitive).
        var entity = new ResourceServer("external-id", "audience", "name", 3600)
        {
            Id = "id",
            Scopes = scopeModels.Take(2).Select(scope => new Scope("id", scope.Value.ToUpper(), scope.Description?.ToUpper())).ToList()
        };
        
        var command = GetRequest(entity.Id, entity.Name, entity.TokenLifetime, scopeModels);

        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(entity);

        var sut = CreateSut();
        var actual = await sut.Handle(command, default);
        actual.Scopes.Should().HaveCount(3);

        await _idpService.Received(1).UpdateResourceServerAsync(entity, Arg.Any<CancellationToken>());
        await _repository.Received(1).UpdateAsync(actual, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    /*
     * Private methods
     */
    
    private PatchResourceServerCommandHandler CreateSut()
    {
        _unitOfWork.ResourceServers.Returns(_repository);
        return new PatchResourceServerCommandHandler(_unitOfWork, _idpService, _validator, _logger);
    }
    
    private void SetupValidator(bool isSuccess) 
        => _validator.ValidateAsync(Arg.Any<PatchResourceServerCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(isSuccess 
                ? []
                : new[] { new ValidationFailure("propertyName", "errorMessage") }));

    private static PatchResourceServerCommand GetRequest(
        string id = "id", 
        string name = "name", 
        int tokenLifetime = 3600, 
        ICollection<RequestScopeModel>? scopes = null)
        => new(id, name, tokenLifetime, scopes);
}