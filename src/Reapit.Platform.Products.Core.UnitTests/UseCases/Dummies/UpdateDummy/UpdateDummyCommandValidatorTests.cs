using Reapit.Platform.Products.Core.UseCases.Dummies.UpdateDummy;
using Reapit.Platform.Products.Data.Repositories;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Dummies.UpdateDummy;

public class UpdateDummyCommandValidatorTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IDummyRepository _dummyRepository = Substitute.For<IDummyRepository>();
    
    [Fact]
    public async Task Validation_Succeeds_WhenModelValid()
    {
        var model = GetCommand();
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(model);
        actual.IsValid.Should().BeTrue();
    }
    
    /*
     * Id
     */

    [Fact]
    public async Task Validation_Fails_WhenIdNull()
    {
        var model = GetCommand(string.Empty);

        var sut = CreateSut();
        var actual = await sut.ValidateAsync(model);
        actual.IsValid.Should().BeFalse();
        actual.Errors.Should().HaveCount(1)
            .And.AllSatisfy(failure => failure.PropertyName.Should().Be(nameof(UpdateDummyCommand.Id)));
    }
    
    [Fact]
    public async Task Validation_Fails_WhenIdIsNotValidGuid()
    {
        var model = GetCommand("not a valid guid");

        var sut = CreateSut();
        var actual = await sut.ValidateAsync(model);
        actual.IsValid.Should().BeFalse();
        actual.Errors.Should().HaveCount(1)
            .And.AllSatisfy(failure => failure.PropertyName.Should().Be(nameof(UpdateDummyCommand.Id)));
    }
    
    /*
     * Name
     */
    
    [Fact]
    public async Task Validation_Fails_WhenNameIsEmpty()
    {
        var command = GetCommand(name: string.Empty);
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.IsValid.Should().BeFalse();
        actual.Errors.Should().HaveCount(1)
            .And.AllSatisfy(failure => failure.PropertyName.Should().BeEquivalentTo(nameof(UpdateDummyCommand.Name)));

        await ValidateGetAsyncCalled(Arg.Any<string?>(), 0);
    }
    
    [Fact]
    public async Task Validation_Fails_WhenNameExceedsMaxLength()
    {
        var command = GetCommand(name: new string('a', 101));
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.IsValid.Should().BeFalse();
        actual.Errors.Should().HaveCount(1)
            .And.AllSatisfy(failure => failure.PropertyName.Should().BeEquivalentTo(nameof(UpdateDummyCommand.Name)));

        await ValidateGetAsyncCalled(Arg.Any<string?>(), 0);
    }
    
    [Fact]
    public async Task Validation_Fails_WhenNameIsNotUnique()
    {
        var id = Guid.NewGuid();
        var command = GetCommand(id: $"{id:N}", name: "test-name");
        SetupGetAsync(command.Name, new Dummy("test-name"){ Id = Guid.NewGuid() });
        
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.IsValid.Should().BeFalse();
        actual.Errors.Should().HaveCount(1)
            .And.AllSatisfy(failure => failure.PropertyName.Should().BeEquivalentTo(nameof(UpdateDummyCommand.Name)));

        await ValidateGetAsyncCalled(Arg.Any<string?>(), 1);
    }
    
    [Fact]
    public async Task Validation_Succeeds_WhenOnlyExistingUseOfNameIsSameDummy()
    {
        var id = Guid.NewGuid();
        var command = GetCommand(id: $"{id:N}", name: "test-name");
        SetupGetAsync(command.Name, new Dummy("test-name"){ Id = id });
        
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(command);
        actual.IsValid.Should().BeTrue();
        await ValidateGetAsyncCalled(Arg.Any<string?>(), 1);
    }
    
    /*
     * Private methods
     */

    private UpdateDummyCommandValidator CreateSut()
    {
        _unitOfWork.Dummies.Returns(_dummyRepository);
        return new UpdateDummyCommandValidator(_unitOfWork);
    }

    private static UpdateDummyCommand GetCommand(string? id = null, string name = "test-name") 
        => new(id ?? Guid.NewGuid().ToString("N"), name);
    
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