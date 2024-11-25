using System.Net;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Reapit.Platform.Products.Api.Controllers.Dummies.V1;
using Reapit.Platform.Products.Api.Controllers.Dummies.V1.Models;
using Reapit.Platform.Products.Api.IntegrationTests.TestHelpers;
using Reapit.Platform.Products.Data.Context;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Api.IntegrationTests.Controllers.Dummies;

public class DummyControllerTests : IClassFixture<TestApiFactory>
{
    private const string ApiVersionHeader = "x-api-version";
    
    private readonly TestApiFactory _factory;
    private readonly IMapper _mapper = new MapperConfiguration(cfg 
            => cfg.AddProfile<DummyProfile>())
        .CreateMapper();

    public DummyControllerTests(TestApiFactory factory)
        => _factory = factory;
    
    /*
     * GET /api/dummy
     */
    
    [Fact]
    public async Task GetDummies_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        await InitializeDatabaseAsync();
        
        const string url = "/api/dummy";
        var client = _factory.CreateClient();

        var response = await client.GetAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task GetDummies_ReturnsBadRequest_WhenOlderApiVersionProvided()
    {
        await InitializeDatabaseAsync();
        
        const string url = "/api/dummy";
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiVersionHeader, "0.5");

        var response = await client.GetAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task GetDummies_ReturnsOk_WhenLaterSupportedApiVersionProvided()
    {
        await InitializeDatabaseAsync();
        
        const string url = "/api/dummy?createdFrom=2021-03-03T00:00:00Z";
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiVersionHeader, "1.99");

        var response = await client.GetAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task GetDummies_ReturnsOk_WhenRequestSuccessful()
    {
        await InitializeDatabaseAsync();
        
        const string url = "/api/dummy";
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiVersionHeader, "1.0");

        var response = await client.GetAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<ReadDummyResponseModel>>();
        
        var expected = _mapper.Map<IEnumerable<ReadDummyResponseModel>>(SeedData);
        result.Should().BeEquivalentTo(expected);
    }
    
    /*
     * GET /api/dummy/{id}
     */
    
    [Fact]
    public async Task GetDummyById_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        await InitializeDatabaseAsync();
        
        var url = $"/api/dummy/{Guid.NewGuid():N}";
        var client = _factory.CreateClient();

        var response = await client.GetAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task GetDummyById_ReturnsNotFound_WhenDummyDoesNotExist()
    {
        await InitializeDatabaseAsync();
        
        var url = $"/api/dummy/{Guid.Empty:N}";
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiVersionHeader, "1.0");

        var response = await client.GetAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task GetDummyById_ReturnsUnprocessable_WhenIdIsNotAGuid()
    {
        await InitializeDatabaseAsync();
        
        var url = "/api/dummy/arbitrary";
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiVersionHeader, "1.0");

        var response = await client.GetAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task GetDummyById_ReturnsOk_WhenDummyExists()
    {
        await InitializeDatabaseAsync();

        var id = 4.ToGuid();
        var expected = _mapper.Map<ReadDummyResponseModel>(SeedData.Single(item => item.Id.Equals(id)));
        
        var url = $"/api/dummy/{id:D}";
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiVersionHeader, "1.0");
        
        var response = await client.GetAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ReadDummyResponseModel>();
        result.Should().BeEquivalentTo(expected);
    }
    
    /*
     * POST /api/dummy
     */
    
    [Fact]
    public async Task CreateDummy_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        await InitializeDatabaseAsync();
        
        const string url = "/api/dummy";
        var client = _factory.CreateClient();
        
        var requestBody = new WriteDummyRequestModel("dummy-name"); 
        var response = await client.PostAsync(url, GetStringContent(requestBody));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task CreateDummy_ReturnsUnprocessable_WhenValidationFailed()
    {
        await InitializeDatabaseAsync();
        
        const string url = "/api/dummy";
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiVersionHeader, "1.0");

        var requestBody = new WriteDummyRequestModel(new string('a', 101));
        var response = await client.PostAsync(url, GetStringContent(requestBody));
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task CreateDummy_ReturnsCreated_WhenDummyCreated()
    {
        await InitializeDatabaseAsync();
        
        const string url = "/api/dummy";
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiVersionHeader, "1.0");

        var requestBody = new WriteDummyRequestModel("Dummy Six");
        var response = await client.PostAsync(url, GetStringContent(requestBody));
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<ReadDummyResponseModel>();
        result!.Name.Should().BeEquivalentTo(requestBody.Name);

        response.Headers.Location!.ToString().Should().EndWith($"/api/Dummy/{result.Id}");
    }
    
    /*
     * Put /api/dummy/{id}
     */
    
    [Fact]
    public async Task UpdateDummy_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        await InitializeDatabaseAsync();

        var id = 3.ToGuid();
        var body = new WriteDummyRequestModel("Dummy Three - Updated");
        
        var url = $"/api/dummy/{id:N}";
        var client = _factory.CreateClient();

        var response = await client.PutAsync(url, GetStringContent(body));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task UpdateDummy_ReturnsUnprocessable_WhenValidationFailed()
    {
        await InitializeDatabaseAsync();
        
        var body = new WriteDummyRequestModel("Dummy Four - Updated");
        
        const string url = "/api/dummy/arbitrary";
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiVersionHeader, "1.0");

        var response = await client.PutAsync(url, GetStringContent(body));
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task UpdateDummy_ReturnsNotFound_WhenDummyDoesNotExist()
    {
        await InitializeDatabaseAsync();

        var id = 6.ToGuid();
        var body = new WriteDummyRequestModel("Dummy Six - Doesn't Exist");
        
        var url = $"/api/dummy/{id:N}";
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiVersionHeader, "1.0");

        var response = await client.PutAsync(url, GetStringContent(body));
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task UpdateDummy_ReturnsOk_WhenDummyUpdated()
    {
        await InitializeDatabaseAsync();

        var id = 4.ToGuid();
        var body = new WriteDummyRequestModel("Dummy Four - Updated");
        
        var url = $"/api/dummy/{id:D}";
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiVersionHeader, "1.0");

        var response = await client.PutAsync(url, GetStringContent(body));
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ReadDummyResponseModel>();
        result!.Name.Should().BeEquivalentTo(body.Name);
    }
    
    /*
     * GET /api/dummy/{id}
     */
    
    [Fact]
    public async Task DeleteDummy_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        await InitializeDatabaseAsync();
        
        var url = $"/api/dummy/{Guid.NewGuid():N}";
        var client = _factory.CreateClient();

        var response = await client.DeleteAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task DeleteDummy_ReturnsNotFound_WhenDummyDoesNotExist()
    {
        await InitializeDatabaseAsync();
        
        var url = $"/api/dummy/{Guid.Empty:N}";
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiVersionHeader, "1.0");

        var response = await client.DeleteAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeleteDummy_ReturnsUnprocessable_WhenIdIsNotAGuid()
    {
        await InitializeDatabaseAsync();
        
        const string url = "/api/dummy/arbitrary";
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiVersionHeader, "1.0");

        var response = await client.DeleteAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }
    
    [Fact]
    public async Task DeleteDummy_ReturnsOk_WhenDummyExists()
    {
        await InitializeDatabaseAsync();

        var id = 4.ToGuid();
        
        var url = $"/api/dummy/{id:D}";
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiVersionHeader, "1.0");
        
        var response = await client.DeleteAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    /*
     * Private methods
     */

    private async Task InitializeDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var dbContext = scopedServices.GetRequiredService<DemoDbContext>();

        // Make sure it's created
        _ = await dbContext.Database.EnsureCreatedAsync();
        
        // Make sure it's empty
        dbContext.Dummies.RemoveRange(dbContext.Dummies);
        
        // Populate with seed data
        await dbContext.Dummies.AddRangeAsync(SeedData);
        
        // Save the changes
        _ = await dbContext.SaveChangesAsync();
    }

    private static StringContent GetStringContent(object payload)
        => new(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
    
    private static IEnumerable<Dummy> SeedData
        => new Dummy[]
        {
            new()
            {
                Id = 1.ToGuid(),
                Name = "Dummy One",
                DateCreated = new DateTime(2021, 1, 1, 1, 1, 1, DateTimeKind.Utc),
                DateModified = new DateTime(2022, 1, 1, 1, 1, 1, DateTimeKind.Utc)
            },
            new()
            {
                Id = 2.ToGuid(),
                Name = "Dummy Two",
                DateCreated = new DateTime(2021, 2, 2, 2, 2, 2, DateTimeKind.Utc),
                DateModified = new DateTime(2022, 2, 2, 2, 2, 2, DateTimeKind.Utc)
            },
            new()
            {
                Id = 3.ToGuid(),
                Name = "Dummy Three",
                DateCreated = new DateTime(2021, 3, 3, 3, 3, 3, DateTimeKind.Utc),
                DateModified = new DateTime(2022, 3, 3, 3, 3, 3, DateTimeKind.Utc)
            },
            new()
            {
                Id = 4.ToGuid(),
                Name = "Dummy Four",
                DateCreated = new DateTime(2021, 4, 4, 4, 4, 4, DateTimeKind.Utc),
                DateModified = new DateTime(2022, 4, 4, 4, 4, 4, DateTimeKind.Utc)
            },
            new()
            {
                Id = 5.ToGuid(),
                Name = "Dummy Five",
                DateCreated = new DateTime(2021, 5, 5, 5, 5, 5, DateTimeKind.Utc),
                DateModified = new DateTime(2022, 5, 5, 5, 5, 5, DateTimeKind.Utc)
            }
        };
}