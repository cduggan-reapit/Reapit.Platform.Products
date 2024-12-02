using Reapit.Platform.Products.Core.UseCases;
using Reapit.Platform.Products.Core.UseCases.Applications;
using Reapit.Platform.Products.Core.UseCases.Applications.PatchApplication;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Repositories.Apps;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;
using Command = Reapit.Platform.Products.Core.UseCases.Applications.PatchApplication.PatchApplicationCommand;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Applications.PatchApplication;

public class PatchApplicationCommandValidatorTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IAppRepository _repository = Substitute.For<IAppRepository>();
    
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
        var entity = new App("name", "description", false);
        var request = GetRequest(entity.Id, "valid name", "valid description");
        
        _repository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(entity);

        _repository.GetAsync()
            .ReturnsForAnyArgs(Array.Empty<App>());
        
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
        var request = GetRequest(name: new string('a', 101));
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.Name), ApplicationValidationMessages.NameTooLong);
        
        await _repository.DidNotReceive().GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Validate_ReturnsSuccess_WhenNameUnavailable_ButAppNotFound()
    {
        const string name = "crooked coast";
        var request = GetRequest(name: name);

        _repository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<App?>(null));
        
        _repository.GetAsync(name: name, pagination: new PaginationFilter(PageSize: 1), cancellationToken: Arg.Any<CancellationToken>())
            .Returns([new App("conflicting", "app", true)]);
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Pass();
    }
    
    [Fact]
    public async Task Validate_ReturnsSuccess_WhenNameUnchanged()
    {
        const string name = "june rest";
        var request = GetRequest(name: name);
        var entity = new App("june rest", "not-validated", true);

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
        const string name = "bearings";
        var request = GetRequest(name: name);
        var entity = new App("different-name", "description", false);

        _repository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(entity);
        
        _repository.GetAsync(name: name, pagination: new PaginationFilter(PageSize: 1), cancellationToken: Arg.Any<CancellationToken>())
            .Returns([new App("conflicting", "app", false)]);
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.Name), CommonValidationMessages.Unique);
    }
    
    /*
     * Description
     */
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenDescriptionTooLong()
    {
        var request = GetRequest(description: new string('a', 1001));
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.Description), ApplicationValidationMessages.DescriptionTooLong);
    }
        
    /*
     * Private methods
     */

    private PatchApplicationCommandValidator CreateSut()
    {
        _unitOfWork.Apps.Returns(_repository);
        return new PatchApplicationCommandValidator(_unitOfWork);
    }
    
    private static Command GetRequest(
        string id = "id",
        string name = "name",
        string description = "description")
        => new(id, name, description);
}