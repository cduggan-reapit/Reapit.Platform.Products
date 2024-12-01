using Reapit.Platform.Products.Core.UseCases;
using Reapit.Platform.Products.Core.UseCases.Common.Scopes;
using Reapit.Platform.Products.Core.UseCases.ResourceServers;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.UpdateResourceServer;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Repositories.ResourceServers;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;
using Command = Reapit.Platform.Products.Core.UseCases.ResourceServers.UpdateResourceServer.UpdateResourceServerCommand;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.ResourceServers.UpdateResourceServer;

public class UpdateResourceServerCommandValidatorTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IResourceServerRepository _repository = Substitute.For<IResourceServerRepository>();
    
    [Fact]
    public async Task Validate_ReturnsSuccess_WhenPropertiesNull()
    {
        var request = GetRequest();
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Pass();
    } 
    
    [Fact]
    public async Task Validate_ReturnsSuccess_WhenPropertiesValid()
    {
        var request = GetRequest(name: "new value", tokenLifetime: 7200, scopes: [new RequestScopeModel("scope.one", null)]);
        var entity = new ResourceServer("not-validated", "not-validated", "name", 3600);

        _repository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(entity);

        _repository.GetAsync()
            .ReturnsForAnyArgs(Array.Empty<ResourceServer>());
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Pass();
    } 
    
    /*
     * Name
     */
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenNameIsEmpty()
    {
        var request = GetRequest(name: string.Empty);
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.Name), CommonValidationMessages.Required);
        
        await _repository.DidNotReceive().GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenNameTooLong()
    {
        var request = GetRequest(name: new string('a', 201));
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.Name), ResourceServerValidationMessages.NameTooLong);
        
        await _repository.DidNotReceive().GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenNameContainsInvalidCharacters()
    {
        var request = GetRequest(name: "<name>");
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.Name), ResourceServerValidationMessages.NameInvalid);
        
        await _repository.DidNotReceive().GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Validate_ReturnsSuccess_WhenNameUnavailable_ButResourceServerNotFound()
    {
        const string name = "architects";
        var request = GetRequest(name: name);

        _repository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<ResourceServer?>(null));
        
        _repository.GetAsync(name: name, pagination: new PaginationFilter(PageSize: 1), cancellationToken: Arg.Any<CancellationToken>())
            .Returns([new ResourceServer("conflicting", "resource", "server", 1)]);
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Pass();
    }
    
    [Fact]
    public async Task Validate_ReturnsSuccess_WhenNameUnchanged()
    {
        const string name = "title fight";
        var request = GetRequest(name: name);
        var entity = new ResourceServer("not-validated", "not-validated", name, 3600);

        _repository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(entity);
        
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Pass();

        await _repository.DidNotReceiveWithAnyArgs().GetAsync();

    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenNameUnavailable()
    {
        const string name = "knocked loose";
        var request = GetRequest(name: name);
        var entity = new ResourceServer("not-validated", "not-validated", "different-name", 3600);

        _repository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(entity);
        
        _repository.GetAsync(name: name, pagination: new PaginationFilter(PageSize: 1), cancellationToken: Arg.Any<CancellationToken>())
            .Returns([new ResourceServer("conflicting", "resource", "server", 1)]);
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.Name), CommonValidationMessages.Unique);
    }
    
    /*
     * TokenLifetime
     */
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenTokenLifetimeTooShort()
    {
        var request = GetRequest(tokenLifetime: 59);
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.TokenLifetime), ResourceServerValidationMessages.TokenLifetimeOutOfRange);
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenTokenLifetimeTooLong()
    {
        var request = GetRequest(tokenLifetime: 86401);
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.TokenLifetime), ResourceServerValidationMessages.TokenLifetimeOutOfRange);
    }

    /*
     * Scopes
     */

    [Fact]
    public async Task Validate_ReturnsFailure_WhenScopeValidatorFails()
    {
        var scope = new RequestScopeModel(new string('a', 281), null);
        var request = GetRequest(scopes: [scope]);
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        
        // Since it's a sub-validator, checking the name can be a pain. We know what it'll be, we can live with the magic string.
        result.Should().Fail("Scopes[0].Value", RequestScopeModelValidationMessages.ValueTooLong);
    }
    
    /*
     * Private methods
     */

    private UpdateResourceServerCommandValidator CreateSut()
    {
        _unitOfWork.ResourceServers.Returns(_repository);
        return new UpdateResourceServerCommandValidator(_unitOfWork);
    }
    
    private static Command GetRequest(
        string id = "id",
        string? name = null,
        int? tokenLifetime = null,
        ICollection<RequestScopeModel>? scopes = null)
        => new(id, name, tokenLifetime, scopes ?? []);
}