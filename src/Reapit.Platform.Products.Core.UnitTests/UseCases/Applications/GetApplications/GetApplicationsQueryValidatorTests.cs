using Reapit.Platform.Products.Core.UseCases;
using Reapit.Platform.Products.Core.UseCases.Applications.GetApplications;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Applications.GetApplications;

public class GetApplicationsQueryValidatorTests
{
    [Fact]
    public async Task Validate_Succeeds_WhenRequestValid()
    {
        var request = GetRequest(1);
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Pass();
    }
    
    /*
     * Cursor
     */
    
    [Fact]
    public async Task Validate_Succeeds_WhenCursorLessThanZero()
    {
        var request = GetRequest(-1);
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(GetApplicationsQuery.Cursor), CommonValidationMessages.CursorOutOfRange);
    }
    
    /*
     * PageSize
     */
    
    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public async Task Validate_Succeeds_WhenPageSizeOutOfRange(int pageSize)
    {
        var request = GetRequest(pageSize: pageSize);
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail(nameof(GetApplicationsQuery.PageSize), CommonValidationMessages.PageSizeOutOfRange);
    }
    
    /*
     * Private methods
     */

    private static GetApplicationsQueryValidator CreateSut() => new();

    private static GetApplicationsQuery GetRequest(long? cursor = null, int pageSize = 25)
        => new(cursor, pageSize);
}