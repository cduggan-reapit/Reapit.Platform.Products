using Reapit.Platform.Products.Core.UseCases;
using Reapit.Platform.Products.Core.UseCases.Applications;
using Reapit.Platform.Products.Core.UseCases.Applications.CreateApplication;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Repositories.Apps;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;
using Command = Reapit.Platform.Products.Core.UseCases.Applications.CreateApplication.CreateApplicationCommand;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Applications.CreateApplication;

public class CreateApplicationCommandValidatorTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IAppRepository _repository = Substitute.For<IAppRepository>();

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
        var request = GetRequest(name: new string('a', 101));
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(Command.Name), ApplicationValidationMessages.NameTooLong);
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenNameUnavailable()
    {
        const string name = "a day to remember";
        var request = GetRequest(name: name);

        _repository.GetAsync(name: name, pagination: new PaginationFilter(PageSize: 1), cancellationToken: Arg.Any<CancellationToken>())
            .Returns([new App("name", "description", true)]);
        
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

    private CreateApplicationCommandValidator CreateSut()
    {
        _unitOfWork.Apps.Returns(_repository);
        return new CreateApplicationCommandValidator(_unitOfWork);
    }
    
    private static Command GetRequest(
        string name = "name",
        string description = "description",
        bool isFirstParty = false)
        => new(name, description, isFirstParty);
}