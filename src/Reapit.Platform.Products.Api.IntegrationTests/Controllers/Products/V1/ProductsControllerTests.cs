using System.Net;
using AutoMapper;
using Reapit.Platform.Products.Api.Controllers.Products.V1;
using Reapit.Platform.Products.Api.Controllers.Products.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Data.Context;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Api.IntegrationTests.Controllers.Products.V1;

public class ProductsControllerTests(TestApiFactory apiFactory) : ApiIntegrationTestBase(apiFactory)
{
    // Services
    private readonly IMapper _mapper = new MapperConfiguration(cfg 
            => cfg.AddProfile<ProductsProfile>())
        .CreateMapper();
    
    // Setup
    private const string BaseUrl = "/api/products";
    private static readonly ICollection<Product> SeedData = SeedHelpers.GetProductSeedData(200, 2);
    
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
        var expected = _mapper.Map<ResultPage<ProductModel>>(SeedData.Take(3));
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
    public async Task GetById_ReturnsNotFound_WhenPropertyDoesNotExist()
    {
        const string url = $"{BaseUrl}/missing";
        var response = await SendRequestAsync(HttpMethod.Get, url);
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenRequestSuccessful()
    {
        await InitializeDatabaseAsync();
        var id = 97.AsIdentity();
        var expected = _mapper.Map<ProductDetailsModel>(SeedData.Single(item => item.Id == id));
        
        var url = $"{BaseUrl}/{id}";
        var response = await SendRequestAsync(HttpMethod.Get, url);
        await response.Should().HaveStatusCode(HttpStatusCode.OK)
            .And.HavePayloadAsync(expected);
    }
    
    /*
     * POST /
     */
    
    [Fact]
    public async Task Post_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        var content = new CreateProductRequestModel("name", "description");
        var response = await SendRequestAsync(HttpMethod.Post, BaseUrl, null, content);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }
    
    [Fact]
    public async Task Post_ReturnsBadRequest_WhenApiVersionUnsupported()
    {
        var content = new CreateProductRequestModel("name", "description");
        var response = await SendRequestAsync(HttpMethod.Post, BaseUrl, "0.9", content);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task Post_ReturnsUnprocessable_WhenValidationFails()
    {
        var content = new CreateProductRequestModel(new string('a', 101), "description");
        var response = await SendRequestAsync(HttpMethod.Post, BaseUrl, content: content);
        await response.Should().HaveStatusCode(HttpStatusCode.UnprocessableContent)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ValidationFailed);
    }

    [Fact]
    public async Task Post_ReturnsCreated_WhenRequestSuccessful()
    {
        await InitializeDatabaseAsync();
        var content = new CreateProductRequestModel("new product name", "new product description");
        var response = await SendRequestAsync(HttpMethod.Post, BaseUrl, content: content);
        await response.Should().HaveStatusCode(HttpStatusCode.Created)
            .And.MatchPayloadAsync<ProductModel>(actual => actual.Name == content.Name);
    }
    
    /*
     * PATCH /{id}
     */
    
    [Fact]
    public async Task Patch_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        const string url = $"{BaseUrl}/any";
        var content = new PatchProductRequestModel("name", "description");
        var response = await SendRequestAsync(HttpMethod.Patch, url, null, content);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }
    
    [Fact]
    public async Task Patch_ReturnsBadRequest_WhenApiVersionUnsupported()
    {
        const string url = $"{BaseUrl}/any";
        var content = new PatchProductRequestModel("name", "description");
        var response = await SendRequestAsync(HttpMethod.Patch, url, "0.9", content);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }

    [Fact]
    public async Task Patch_ReturnsUnprocessable_WhenValidationFails()
    {
        const string url = $"{BaseUrl}/any";
        var content = new PatchProductRequestModel(new string('a', 101), "description");
        var response = await SendRequestAsync(HttpMethod.Patch, url, content: content);
        await response.Should().HaveStatusCode(HttpStatusCode.UnprocessableContent)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ValidationFailed);
    }

    [Fact]
    public async Task Patch_ReturnsNotFound_WhenProductDoesNotExist()
    {
        const string url = $"{BaseUrl}/missing";
        var content = new PatchProductRequestModel("name", "description");
        var response = await SendRequestAsync(HttpMethod.Patch, url, content: content);
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }

    [Fact]
    public async Task Patch_ReturnsNoContent_WhenRequestSuccessful()
    {
        await InitializeDatabaseAsync();
        var id = 62.AsIdentity();
        var url = $"{BaseUrl}/{id}";
        var content = new PatchProductRequestModel("modified product name", "modified product description");
        var response = await SendRequestAsync(HttpMethod.Patch, url, content: content);
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);
    }
    
    /*
     * PATCH /{id}
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
    public async Task Delete_ReturnsNotFound_WhenProductDoesNotExist()
    {
        const string url = $"{BaseUrl}/missing";
        var response = await SendRequestAsync(HttpMethod.Delete, url);
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenRequestSuccessful()
    {
        await InitializeDatabaseAsync();
        var id = 62.AsIdentity();
        var url = $"{BaseUrl}/{id}";
        var response = await SendRequestAsync(HttpMethod.Delete, url);
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);
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
        await dbContext.Products.AddRangeAsync(SeedData);
        
        _ = await dbContext.SaveChangesAsync();
    }
    
    
}