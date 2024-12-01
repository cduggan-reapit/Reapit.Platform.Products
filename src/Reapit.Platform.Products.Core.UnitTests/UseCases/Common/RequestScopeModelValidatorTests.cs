using Reapit.Platform.Products.Core.UseCases;
using Reapit.Platform.Products.Core.UseCases.Common.Scopes;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Common;

public class RequestScopeModelValidatorTests
{
    [Fact]
    public async Task Validate_ReturnsSuccess_WhenModelValid()
    {
        var model = GetRequest();
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(model);
        actual.Should().Pass();
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenValueEmpty()
    {
        var model = GetRequest(value: "");
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(model);
        actual.Should().Fail(nameof(RequestScopeModel.Value), CommonValidationMessages.Required);
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenValueTooLong()
    {
        var model = GetRequest(value: new string('a', 300));
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(model);
        actual.Should().Fail(nameof(RequestScopeModel.Value), RequestScopeModelValidationMessages.ValueTooLong);
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenDescriptionTooLong()
    {
        var model = GetRequest(description: new string('a', 501));
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(model);
        actual.Should().Fail(nameof(RequestScopeModel.Description), RequestScopeModelValidationMessages.DescriptionTooLong);
    }
    
    /*
     * Private methods
     */
    
    private static RequestScopeModelValidator CreateSut() => new();

    private static RequestScopeModel GetRequest(string value = "value", string description = "description")
        => new(value, description);
}