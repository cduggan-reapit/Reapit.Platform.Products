using Reapit.Platform.Products.Core.UseCases;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.Shared;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.ResourceServers.Shared;

public class ResourceServerRequestScopeModelValidatorTests
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
        actual.Should().Fail(nameof(ResourceServerRequestScopeModel.Value), CommonValidationMessages.Required);
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenValueTooLong()
    {
        var model = GetRequest(value: new string('a', 300));
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(model);
        actual.Should().Fail(nameof(ResourceServerRequestScopeModel.Value), ResourceServerRequestScopeValidationMessages.ValueTooLong);
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenDescriptionTooLong()
    {
        var model = GetRequest(description: new string('a', 501));
        var sut = CreateSut();
        var actual = await sut.ValidateAsync(model);
        actual.Should().Fail(nameof(ResourceServerRequestScopeModel.Description), ResourceServerRequestScopeValidationMessages.DescriptionTooLong);
    }
    
    /*
     * Private methods
     */
    
    private static ResourceServerRequestScopeModelValidator CreateSut() => new();

    private static ResourceServerRequestScopeModel GetRequest(string value = "value", string description = "description")
        => new(value, description);
}