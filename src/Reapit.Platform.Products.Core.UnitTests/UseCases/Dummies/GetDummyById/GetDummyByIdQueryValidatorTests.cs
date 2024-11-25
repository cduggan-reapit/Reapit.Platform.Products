using Reapit.Platform.Products.Core.UseCases.Dummies.GetDummyById;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Dummies.GetDummyById;

public class GetDummyByIdQueryValidatorTests
{
    [Fact]
    public async Task Validator_Succeeds_WhenModelValid()
    {
        var model = new GetDummyByIdQuery($"{Guid.NewGuid():N}");

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
        var model = new GetDummyByIdQuery(null!);

        var sut = CreateSut();
        var actual = await sut.ValidateAsync(model);
        actual.IsValid.Should().BeFalse();
        actual.Errors.Should().HaveCount(1)
            .And.AllSatisfy(failure => failure.PropertyName.Should().Be(nameof(GetDummyByIdQuery.Id)));
    }
    
    [Fact]
    public async Task Validator_Fails_WhenIdIsNotValidGuid()
    {
        var model = new GetDummyByIdQuery("not a valid guid");

        var sut = CreateSut();
        var actual = await sut.ValidateAsync(model);
        actual.IsValid.Should().BeFalse();
        actual.Errors.Should().HaveCount(1)
            .And.AllSatisfy(failure => failure.PropertyName.Should().Be(nameof(GetDummyByIdQuery.Id)));
    }
    
    /*
     * Private methods
     */

    private static GetDummyByIdQueryValidator CreateSut() => new();
}