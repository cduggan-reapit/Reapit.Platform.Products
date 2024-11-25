using System.Net;

namespace Reapit.Platform.Products.Api.IntegrationTests.Controllers;

public class HealthControllerTests : IClassFixture<TestApiFactory>
{
    private readonly TestApiFactory _factory;

    public HealthControllerTests(TestApiFactory factory)
        => _factory = factory;
    
    /*
     * GET /api/health
     */
    
    [Fact]
    public async Task HealthCheck_ReturnsNoContent_WhenRequestSuccessful()
    {
        const string url = "/api/health";
        var client = _factory.CreateClient();
        var response = await client.GetAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}