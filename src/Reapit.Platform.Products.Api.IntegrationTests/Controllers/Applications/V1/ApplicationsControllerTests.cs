using System.Net;
using AutoMapper;
using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Api.Controllers.Applications.V1;
using Reapit.Platform.Products.Api.Controllers.Applications.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Data.Context;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Api.IntegrationTests.Controllers.Applications.V1;

public class ApplicationsControllerTests(TestApiFactory apiFactory) : ApiIntegrationTestBase(apiFactory)
{
    private static readonly DateTimeOffset BaseDate = DateTime.Parse("2020-01-01T12:00:00Z");
    private readonly IMapper _mapper = new MapperConfiguration(
            cfg => cfg.AddProfile<ApplicationsProfile>())
        .CreateMapper();

    private const string BaseUrl = "/api/applications";
    
    /*
     * GET / => GetApplications
     */
    
    [Fact]
    public async Task GetApplications_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Get, BaseUrl, null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }

    [Fact]
    public async Task GetApplications_ReturnsBadRequest_WhenApiVersionNotSupported()
    {
        var response = await SendRequestAsync(HttpMethod.Get, BaseUrl, "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }

    [Fact]
    public async Task GetApplications_ReturnsBadRequest_WhenQueryStringParametersInvalid()
    {
        var response = await SendRequestAsync(HttpMethod.Get, $"{BaseUrl}?cursor=-1");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.QueryStringInvalid);
    }

    [Fact]
    public async Task GetApplications_ReturnsOk_WhenRequestSuccessful()
    {
        await InitializeDatabaseAsync();
        var expected = _mapper.Map<ResultPage<ApplicationModel>>(SeedData.Take(3));
        var response = await SendRequestAsync(HttpMethod.Get, $"{BaseUrl}?pageSize=3");
        await response.Should().HaveStatusCode(HttpStatusCode.OK).And.HavePayloadAsync(expected);
    }
    
    /*
     * GET /{id} => GetApplicationById
     */
    
    [Fact]
    public async Task GetApplicationById_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Get, BaseUrl, null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }

    [Fact]
    public async Task GetApplicationById_ReturnsBadRequest_WhenApiVersionNotSupported()
    {
        var response = await SendRequestAsync(HttpMethod.Get, BaseUrl, "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task GetApplicationById_ReturnsNotFound_WhenEntityDoesNotExist()
    {
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Get, $"{BaseUrl}/missing");
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound).And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }

    [Fact] 
    public async Task GetApplicationById_ReturnsOk_WhenRequestSuccessful()
    {
        await InitializeDatabaseAsync();
        var entity = SeedData.ElementAt(35);
        var expected = _mapper.Map<ApplicationDetailsModel>(entity);
        
        var response = await SendRequestAsync(HttpMethod.Get, $"{BaseUrl}/{entity.Id}");
        await response.Should().HaveStatusCode(HttpStatusCode.OK)
            .And.HavePayloadAsync(expected);
    }
    
    /*
     * POST / => CreateApplication
     */
    
    [Fact]
    public async Task CreateApplication_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Post, BaseUrl, null, content: GetCreateModel());
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }

    [Fact]
    public async Task CreateApplication_ReturnsBadRequest_WhenApiVersionNotSupported()
    {
        var response = await SendRequestAsync(HttpMethod.Post, BaseUrl, "0.9", content: GetCreateModel());
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task CreateApplication_ReturnsUnprocessable_WhenValidationFailed()
    {
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Post, BaseUrl, content: GetCreateModel(name: new string('a', 101)));
        await response.Should().HaveStatusCode(HttpStatusCode.UnprocessableContent)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ValidationFailed);
    }
    
    [Fact]
    public async Task CreateApplication_ReturnsCreated_WhenEntityCreated()
    {
        var request = GetCreateModel(name: "hot mulligan", description: "end eric sparrow", isFirstParty: true);
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Post, BaseUrl, content: request);
        response.Should().HaveStatusCode(HttpStatusCode.Created);

        // Check what we can in the simple payload
        var created = await response.Content.ReadFromJsonAsync<ApplicationModel>();
        created.Should().Match<ApplicationModel>(model => model.Name == request.Name && model.IsFirstParty == request.IsFirstParty);
        
        // ... and check the rest from the detailed payload
        var check = await SendRequestAsync(HttpMethod.Get, $"{BaseUrl}/{created!.Id}");
        var checkModel = await check.Content.ReadFromJsonAsync<ApplicationDetailsModel>();
        checkModel!.Description.Should().BeEquivalentTo(request.Description);
    }
    
    /*
     * PATCH /{id} => PatchApplication
     */
        
    [Fact]
    public async Task PatchApplication_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Patch, $"{BaseUrl}/any", null, content: GetUpdateModel());
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }

    [Fact]
    public async Task PatchApplication_ReturnsBadRequest_WhenApiVersionNotSupported()
    {
        var response = await SendRequestAsync(HttpMethod.Patch, $"{BaseUrl}/any", "0.9", content: GetUpdateModel());
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task PatchApplication_ReturnsUnprocessable_WhenValidationFailed()
    {
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Patch, $"{BaseUrl}/any", content: GetUpdateModel(name: new string('a', 101)));
        await response.Should().HaveStatusCode(HttpStatusCode.UnprocessableContent)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ValidationFailed);
    }
    
    [Fact]
    public async Task PatchApplication_ReturnsNotFound_WhenEntityDoesNotExist()
    {
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Patch, $"{BaseUrl}/missing", content: GetUpdateModel());
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }
    
    [Fact]
    public async Task PatchApplication_ReturnsNoContent_WhenRequestSuccessful()
    {
        var entity = SeedData.ElementAt(63);
        var request = GetUpdateModel(name: "updated entity name");
        
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Patch, $"{BaseUrl}/{entity.Id}", content: request);
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);
        
        // Quickly check the update took:
        var checkResponse = await SendRequestAsync(HttpMethod.Get, $"{BaseUrl}/{entity.Id}");
        var checkModel = await checkResponse.Content.ReadFromJsonAsync<ApplicationDetailsModel>();
        checkModel!.Name.Should().BeEquivalentTo(request.Name);
        checkModel.Description.Should().BeEquivalentTo(entity.Description);
    }
    
    /*
     * DELETE /{id} => DeleteApplication
     */
    
    [Fact]
    public async Task DeleteApplication_ReturnsBadRequest_WhenNoApiVersionProvided()
    {
        var response = await SendRequestAsync(HttpMethod.Delete, $"{BaseUrl}/any", null);
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnspecifiedApiVersion);
    }

    [Fact]
    public async Task DeleteApplication_ReturnsBadRequest_WhenApiVersionNotSupported()
    {
        var response = await SendRequestAsync(HttpMethod.Delete, $"{BaseUrl}/any", "0.9");
        await response.Should().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDescriptionAsync(ProblemDetailsTypes.UnsupportedApiVersion);
    }
    
    [Fact]
    public async Task DeleteApplication_ReturnsNotFound_WhenEntityDoesNotExist()
    {
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Delete, $"{BaseUrl}/missing");
        await response.Should().HaveStatusCode(HttpStatusCode.NotFound)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ResourceNotFound);
    }
    
    [Fact]
    public async Task DeleteApplication_ReturnsUnprocessable_WhenEntityHasChildren()
    {
        var entity = SeedData.ElementAt(12);
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Delete, $"{BaseUrl}/{entity.Id}");
        await response.Should().HaveStatusCode(HttpStatusCode.UnprocessableContent)
            .And.BeProblemDescriptionAsync(ProblemDetailsTypes.ValidationFailed);
    }
    
    [Fact]
    public async Task DeleteApplication_ReturnsNoContent_WhenRequestSuccessful()
    {
        // Apps which are divisible by 25 (0, 25, 50, 75) don't have any clients:
        var entity = SeedData.ElementAt(75);
        var url = $"{BaseUrl}/{entity.Id}";
        
        await InitializeDatabaseAsync();
        var response = await SendRequestAsync(HttpMethod.Delete, url);
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);

        var check = await SendRequestAsync(HttpMethod.Get, url);
        check.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }
    
    /*
     * Private methods
     */
    
    private static CreateApplicationRequestModel GetCreateModel(
        string name = "audience",
        string description = "name",
        bool isFirstParty = false)
        => new(name, description, isFirstParty);

    private static PatchApplicationRequestModel GetUpdateModel(
        string? name = null,
        string? description = null)
        => new(name, description);
    
    private async Task InitializeDatabaseAsync()
    {
        await using var scope = ApiFactory.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;
        var dbContext = serviceProvider.GetRequiredService<ProductDbContext>();

        _ = await dbContext.Database.EnsureDeletedAsync();
        _ = await dbContext.Database.EnsureCreatedAsync();
        
        dbContext.Apps.AddRange(SeedData);
        
        await dbContext.SaveChangesAsync();
    }

    private static ICollection<Entities.App> SeedData
        => Enumerable.Range(0, 100)
            .Select(seed =>
            {
                var seedGuid = new Guid($"{seed:D32}");
                var seedDate = BaseDate.AddDays(seed);
                using var guidContext = new GuidProviderContext(seedGuid);
                using var timeContext = new DateTimeOffsetProviderContext(seedDate);

                // 1 in 10 apps are first party (those with a seed value divisible by 10)
                var isFirstParty = seed % 10 == 0;
                var app = new Entities.App($"App {seed:D3}", $"Description of App {seed:D3}", isFirstParty);
                
                // Apps which are divisible by 25 (0, 25, 50, 75) don't have any clients:
                if (seed % 25 == 0)
                    return app;

                // All the other apps have two:
                app.Clients.Add(GetClient(seed, ClientType.Machine));
                app.Clients.Add(GetClient(seed, ClientType.AuthCode));
                return app;
            })
            .ToList();

    private static Entities.Client GetClient(int appSeed, ClientType type)
    {
        var appId = $"{appSeed:D32}";
        var seedGuid = new Guid($"{appSeed:D8}-0000-0000-0000-{type.Value:D12}");
        using var guidContext = new GuidProviderContext(seedGuid);

        var seedDate = BaseDate.AddDays(appSeed).AddHours(type.Value);
        using var timeContext = new DateTimeOffsetProviderContext(seedDate);

        return new Entities.Client(appId, string.Empty, type, $"App {appSeed:D3} {type.Name} client", null, null, null, null);
    }
}