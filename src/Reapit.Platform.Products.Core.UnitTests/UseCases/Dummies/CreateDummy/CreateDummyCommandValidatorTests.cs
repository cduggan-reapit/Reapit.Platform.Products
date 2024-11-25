using Reapit.Platform.Products.Core.UseCases.Dummies.CreateDummy;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Dummies.CreateDummy;

public class CreateDummyCommandValidatorTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IDummyRepository _dummyRepository = Substitute.For<IDummyRepository>();

    [Fact]
    public async Task Validation_Succeeds_WhenModelValid_AndNameUnique()
    {
        var command = new CreateDummyCommand("test-name");
        SetupGetAsync(Arg.Any<string>());

        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.IsValid.Should().BeTrue();

        await ValidateGetAsyncCalled(Arg.Any<string>(), 1);
    }
    
    /*
     * Name
     */
    
    [Fact]
    public async Task Validation_Fails_WhenNameIsEmpty()
    {
        var command = new CreateDummyCommand(string.Empty);
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.IsValid.Should().BeFalse();
        actual.Errors.Should().HaveCount(1)
            .And.AllSatisfy(failure => failure.PropertyName.Should().BeEquivalentTo(nameof(CreateDummyCommand.Name)));

        await ValidateGetAsyncCalled(Arg.Any<string?>(), 0);
    }
    
    [Fact]
    public async Task Validation_Fails_WhenNameExceedsMaxLength()
    {
        var command = new CreateDummyCommand(new string('a', 101));
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.IsValid.Should().BeFalse();
        actual.Errors.Should().HaveCount(1)
            .And.AllSatisfy(failure => failure.PropertyName.Should().BeEquivalentTo(nameof(CreateDummyCommand.Name)));

        await ValidateGetAsyncCalled(Arg.Any<string?>(), 0);
    }
    
    [Fact]
    public async Task Validation_Fails_WhenNameIsNotUnique()
    {
        var command = new CreateDummyCommand("test-name");
        SetupGetAsync(command.Name, new Dummy("test-name"));
        
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.IsValid.Should().BeFalse();
        actual.Errors.Should().HaveCount(1)
            .And.AllSatisfy(failure => failure.PropertyName.Should().BeEquivalentTo(nameof(CreateDummyCommand.Name)));

        await ValidateGetAsyncCalled(Arg.Any<string?>(), 1);
    }
    
    /*
     * Private methods
     */

    private CreateDummyCommandValidator CreateSut()
    {
        _unitOfWork.Dummies.Returns(_dummyRepository);
        return new CreateDummyCommandValidator(_unitOfWork);
    }

    private void SetupGetAsync(string? name, params Dummy[] response) 
        => _dummyRepository.GetAsync(
                name: name,
                createdFrom: Arg.Any<DateTime?>(),
                createdTo: Arg.Any<DateTime?>(),
                modifiedFrom: Arg.Any<DateTime?>(),
                modifiedTo: Arg.Any<DateTime?>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(response);

    private async Task ValidateGetAsyncCalled(string? name, int times) 
        => await _dummyRepository.Received(times).GetAsync(
            name: name,
            createdFrom: Arg.Any<DateTime?>(),
            createdTo: Arg.Any<DateTime?>(),
            modifiedFrom: Arg.Any<DateTime?>(),
            modifiedTo: Arg.Any<DateTime?>(),
            cancellationToken: Arg.Any<CancellationToken>());
}