using Reapit.Platform.Products.Core.UseCases;
using Reapit.Platform.Products.Core.UseCases.ResourceServers;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.CreateResourceServer;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.Shared;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Repositories.ResourceServers;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;
using Command = Reapit.Platform.Products.Core.UseCases.ResourceServers.CreateResourceServer.CreateResourceServerCommand;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.ResourceServers.CreateResourceServer;

public class CreateResourceServerCommandValidatorTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IResourceServerRepository _repository = Substitute.For<IResourceServerRepository>();

    [Fact]
    public async Task Validate_ReturnsSuccess_WhenModelValid()
    {
        var request = GetRequest();
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
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenNameTooLong()
    {
        var request = GetRequest(name: new string('a', 201));
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.Name), ResourceServerValidationMessages.NameTooLong);
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenNameContainsInvalidCharacters()
    {
        var request = GetRequest(name: "<name>");
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.Name), ResourceServerValidationMessages.NameInvalid);
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenNameUnavailable()
    {
        const string name = "winona fighter";
        var request = GetRequest(name: name);

        _repository.GetAsync(name: name, pagination: new PaginationFilter(PageSize: 1), cancellationToken: Arg.Any<CancellationToken>())
            .Returns([new ResourceServer("conflicting", "resource", "server", 1)]);
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.Name), CommonValidationMessages.Unique);
    }
    
    /*
     * Audience
     */
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenAudienceIsEmpty()
    {
        var request = GetRequest(audience: string.Empty);
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.Audience), CommonValidationMessages.Required);
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenAudienceTooLong()
    {
        var request = GetRequest(audience: new string('a', 601));
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.Audience), ResourceServerValidationMessages.AudienceTooLong);
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenAudienceUnavailable()
    {
        const string audience = "state champs";
        var request = GetRequest(audience: audience);

        _repository.GetAsync(audience: audience, pagination: new PaginationFilter(PageSize: 1), cancellationToken: Arg.Any<CancellationToken>())
            .Returns([new ResourceServer("conflicting", "resource", "server", 2)]);
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.Audience), CommonValidationMessages.Unique);
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
        var scope = new ResourceServerRequestScopeModel(new string('a', 281), null);
        var request = GetRequest(scopes: [scope]);
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        
        // Since it's a sub-validator, checking the name can be a pain. We know what it'll be, we can live with the magic string.
        result.Should().Fail("Scopes[0].Value", ResourceServerRequestScopeValidationMessages.ValueTooLong);
    }
        
    /*
     * Private methods
     */

    private CreateResourceServerCommandValidator CreateSut()
    {
        _unitOfWork.ResourceServers.Returns(_repository);
        return new CreateResourceServerCommandValidator(_unitOfWork);
    }
    
    private static Command GetRequest(
        string name = "name",
        string audience = "audience",
        int tokenLifetime = 3600,
        ICollection<ResourceServerRequestScopeModel>? scopes = null)
        => new(name, audience, tokenLifetime, scopes ?? []);
}