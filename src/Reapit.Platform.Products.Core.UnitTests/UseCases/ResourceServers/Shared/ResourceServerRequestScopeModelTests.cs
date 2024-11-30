using Auth0.ManagementApi.Models;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.Shared;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.ResourceServers.Shared;

public class ResourceServerRequestScopeModelTests
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
    
    private static ResourceServerRequestScopeModel GetRequest(string value = "value", string? description = "description")
        => new(value, description);
}