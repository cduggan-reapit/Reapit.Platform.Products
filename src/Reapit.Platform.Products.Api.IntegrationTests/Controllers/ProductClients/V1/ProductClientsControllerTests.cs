using System.Net;
using AutoMapper;
using Reapit.Platform.Products.Api.Controllers.ProductClients.V1;
using Reapit.Platform.Products.Api.Controllers.ProductClients.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Products.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Data.Context;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Api.IntegrationTests.Controllers.ProductClients.V1;

public class ProductClientsControllerTests(TestApiFactory apiFactory) : ApiIntegrationTestBase(apiFactory)
{
    // Services
    private readonly IMapper _mapper = new MapperConfiguration(cfg 
            => cfg.AddProfile<ProductClientsProfile>())
        .CreateMapper();
    
    // Setup
    private const string BaseUrl = "/api/product-clients";
    
    private static readonly ICollection<Product> SeedProducts = SeedHelpers.GetProductSeedData(200, 2);
    
    private static readonly ICollection<ProductClient> SeedClients = SeedProducts
        .SelectMany(product => product.Clients)
        .OrderBy(product => product.Cursor)
        .ToList();
    
    /*
     * GET /
     */

    [Fact]
    public async Task Get_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Get, BaseUrl, null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }
    
    [Fact]
    public async Task Get_ReturnsBadRequest_WhenApiVersionUnsupported()
    {
        var response = await SendRequestAsync(HttpMethod.Get, BaseUrl, "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task Get_ReturnsBadRequest_WhenQueryParametersAreInvalid()
    {
        var response = await SendRequestAsync(HttpMethod.Get, $"{BaseUrl}?cursor=-1");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.QueryStringInvalid);
    }

    [Fact]
    public async Task Get_ReturnsOk_WhenRequestSuccessful()
    {
        await InitializeDatabaseAsync();
        var expected = _mapper.Map<ResultPage<ProductDetailsModel>>(SeedClients.Take(3));
        var response = await SendRequestAsync(HttpMethod.Get, $"{BaseUrl}?pageSize=3");
        await response.Should().HaveStatusCode(HttpStatusCode.OK)
            .And.HavePayloadAsync(expected);;
    }
    
    /*
     * GET /{id}
     */
    
    [Fact]
    public async Task GetById_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        const string url = $"{BaseUrl}/any";
        var response = await SendRequestAsync(HttpMethod.Get, url, null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }
    
    [Fact]
    public async Task GetById_ReturnsBadRequest_WhenApiVersionUnsupported()
    {
        const string url = $"{BaseUrl}/any";
        var response = await SendRequestAsync(HttpMethod.Get, url, "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task GetById_ReturnsNotFound_WhenProductClientDoesNotExist()
    {
        await InitializeDatabaseAsync();
        const string url = $"{BaseUrl}/missing";
        var response = await SendRequestAsync(HttpMethod.Get, url);
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenRequestSuccessful()
    {
        await InitializeDatabaseAsync();

        // Each product has two clients (0 & 1)
        var id = GetProductClientId(97, 1);
        var expected = _mapper.Map<ProductClientDetailsModel>(SeedClients.Single(item => item.Id == id));
        
        var url = $"{BaseUrl}/{id}";
        var response = await SendRequestAsync(HttpMethod.Get, url);
        await response.Should().HaveStatusCode(HttpStatusCode.OK)
            .And.HavePayloadAsync(expected);
    }
    
    /*
     * POST /
     */

    private static CreateProductClientRequestModel GetCreateModel(
        string productId, 
        string name = "name", 
        string description = "description", 
        string type = "client_credentials", 
        IEnumerable<string>? callbackUrls = null, 
        IEnumerable<string>? signOutUrls = null)
        => new(productId, name, description, type, callbackUrls, signOutUrls);

    [Fact]
    public async Task Post_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        var content = GetCreateModel(0.AsIdentity());
        var response = await SendRequestAsync(HttpMethod.Post, BaseUrl, null, content);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }
    
    [Fact]
    public async Task Post_ReturnsBadRequest_WhenApiVersionUnsupported()
    {
        var content = GetCreateModel(0.AsIdentity());
        var response = await SendRequestAsync(HttpMethod.Post, BaseUrl, "0.9", content);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task Post_ReturnsUnprocessable_WhenValidationFails()
    {
        var content = GetCreateModel(0.AsIdentity(), name: new string('a', 101));
        var response = await SendRequestAsync(HttpMethod.Post, BaseUrl, content: content);
        await response.Should().HaveStatusCode(HttpStatusCode.UnprocessableContent)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ValidationFailed);
    }

    [Fact]
    public async Task Post_ReturnsCreated_WhenRequestSuccessful()
    {
        await InitializeDatabaseAsync();
        var content = GetCreateModel(0.AsIdentity());
        var response = await SendRequestAsync(HttpMethod.Post, BaseUrl, content: content);
        await response.Should().HaveStatusCode(HttpStatusCode.Created)
            .And.MatchPayloadAsync<ProductModel>(actual => actual.Name == content.Name);
    }
    
    /*
     * PATCH /{id}
     */
    
    private static PatchProductClientRequestModel GetPatchModel(
        string? name = "name", 
        string? description = "description", 
        IEnumerable<string>? callbackUrls = null, 
        IEnumerable<string>? signOutUrls = null)
        => new(name, description, callbackUrls, signOutUrls);
    
    [Fact]
    public async Task Patch_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        const string url = $"{BaseUrl}/any";
        var content = GetPatchModel();
        var response = await SendRequestAsync(HttpMethod.Patch, url, null, content);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }
    
    [Fact]
    public async Task Patch_ReturnsBadRequest_WhenApiVersionUnsupported()
    {
        const string url = $"{BaseUrl}/any";
        var content = GetPatchModel();
        var response = await SendRequestAsync(HttpMethod.Patch, url, "0.9", content);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }

    [Fact]
    public async Task Patch_ReturnsUnprocessable_WhenValidationFails()
    {
        const string url = $"{BaseUrl}/any";
        var content = GetPatchModel(name: new string('a', 101));
        var response = await SendRequestAsync(HttpMethod.Patch, url, content: content);
        await response.Should().HaveStatusCode(HttpStatusCode.UnprocessableContent)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ValidationFailed);
    }

    [Fact]
    public async Task Patch_ReturnsNotFound_WhenProductClientDoesNotExist()
    {
        await InitializeDatabaseAsync();
        const string url = $"{BaseUrl}/missing";
        var content = GetPatchModel();
        var response = await SendRequestAsync(HttpMethod.Patch, url, content: content);
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }

    [Fact]
    public async Task Patch_ReturnsNoContent_WhenRequestSuccessful()
    {
        await InitializeDatabaseAsync();
        var id = GetProductClientId(97, 1);
        var url = $"{BaseUrl}/{id}";
        var content = GetPatchModel(name: "updated name");
        var response = await SendRequestAsync(HttpMethod.Patch, url, content: content);
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);
    }
    
    /*
     * DELETE /{id}
     */
    
    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        const string url = $"{BaseUrl}/any";
        var response = await SendRequestAsync(HttpMethod.Delete, url, null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }
    
    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenApiVersionUnsupported()
    {
        const string url = $"{BaseUrl}/any";
        var response = await SendRequestAsync(HttpMethod.Delete, url, "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenProductClientDoesNotExist()
    {
        await InitializeDatabaseAsync();
        const string url = $"{BaseUrl}/missing";
        var response = await SendRequestAsync(HttpMethod.Delete, url);
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenRequestSuccessful()
    {
        await InitializeDatabaseAsync();
        var id = GetProductClientId(62, 0);
        var url = $"{BaseUrl}/{id}";
        var response = await SendRequestAsync(HttpMethod.Delete, url);
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);
        
        var checkResponse = await SendRequestAsync(HttpMethod.Get, url);
        checkResponse.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }
    
    /*
     * Private methods
     */
    
    private async Task InitializeDatabaseAsync()
    {
        await using var scope = ApiFactory.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        var dbContext = serviceProvider.GetRequiredService<ProductDbContext>();

        _ = await dbContext.Database.EnsureDeletedAsync();
        _ = await dbContext.Database.EnsureCreatedAsync();
        
        // Add seed data
        await dbContext.Products.AddRangeAsync(SeedProducts);
        
        _ = await dbContext.SaveChangesAsync();
    }
    
    private static string GetProductClientId(int productSeed, int clientSeed) 
        => new Guid($"{productSeed:D8}-0000-0000-0000-{clientSeed:D12}").ToString("N");
}