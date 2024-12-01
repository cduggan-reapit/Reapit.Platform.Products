using Auth0.ManagementApi.Models;
using Reapit.Platform.Products.Core.UseCases.Common.Scopes;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Common;

public class RequestScopeModelTests
{
    /*
     * ToEntity
     */

    [Fact]
    public void ToEntity_ReturnsEntity_WithExpectedProperties()
    {
        const string resourceServerId = "resource-server-id";
        var model = GetRequest();

        var expected = new Scope(resourceServerId, model.Value, model.Description);
        var actual = model.ToEntity(resourceServerId);
        actual.Should().BeEquivalentTo(expected);
    } 
    
    /*
     * ToResourceServerScope
     */

    [Fact]
    public void ToResourceServerScope_ReturnsIdPModel_WithExpectedProperties()
    {
        var model = GetRequest();

        var expected = new ResourceServerScope { Value = model.Value,  Description = model.Description };
        var actual = model.ToResourceServerScope();
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ToResourceServerScope_ReturnsIdPModel_WithExpectedProperties_WhenDescriptionNull()
    {
        var model = GetRequest(description: null);

        var expected = new ResourceServerScope { Value = model.Value,  Description = string.Empty };
        var actual = model.ToResourceServerScope();
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * Private methods
     */
    
    private static RequestScopeModel GetRequest(string value = "value", string? description = "description")
        => new(value, description);
}