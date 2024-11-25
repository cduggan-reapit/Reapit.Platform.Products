using Reapit.Platform.Products.Core.UseCases.Dummies.DeleteDummyById;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Dummies.DeleteDummyById;

public class DeleteDummyByIdCommandValidatorTests
{
    [Fact]
    public async Task Validator_Succeeds_WhenModelValid()
    {
        var model = new DeleteDummyByIdCommand($"{Guid.NewGuid():N}");

        var sut = CreateSut();
        var actual = await sut.ValidateAsync(model);
        actual.IsValid.Should().BeTrue();
    }
    
    /*
     * Id
     */

    [Fact]
    public async Task Validator_Fails_WhenIdNull()
    {
        var model = new DeleteDummyByIdCommand(null!);

        var sut = CreateSut();
        var actual = await sut.ValidateAsync(model);
        actual.IsValid.Should().BeFalse();
        actual.Errors.Should().HaveCount(1)
            .And.AllSatisfy(failure => failure.PropertyName.Should().Be(nameof(DeleteDummyByIdCommand.Id)));
    }
    
    [Fact]
    public async Task Validator_Fails_WhenIdIsNotValidGuid()
    {
        var model = new DeleteDummyByIdCommand("not a valid guid");

        var sut = CreateSut();
        var actual = await sut.ValidateAsync(model);
        actual.IsValid.Should().BeFalse();
        actual.Errors.Should().HaveCount(1)
            .And.AllSatisfy(failure => failure.PropertyName.Should().Be(nameof(DeleteDummyByIdCommand.Id)));
    }
    
    /*
     * Private methods
     */

    private static DeleteDummyByIdCommandValidator CreateSut() => new();
}