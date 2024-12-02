using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Api.Controllers.ResourceServers.V1;
using Reapit.Platform.Products.Api.Controllers.ResourceServers.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Api.Extensions;
using Reapit.Platform.Products.Core.UseCases.Common.Scopes;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.CreateResourceServer;
using Reapit.Platform.Products.Core.UseCases.ResourceServers.GetResourceServers;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Api.UnitTests.Controllers.ResourceServers.V1;

public class ResourceServersProfileTests
{
    private readonly IMapper _mapper = new MapperConfiguration(cfg 
            => cfg.AddProfile<ResourceServersProfile>())
        .CreateMapper();
    
    /*
     * GetResourceServersRequestModel => GetResourceServersQuery
     */

    [Fact]
    public void ResourceServersProfile_MapsGetResourceServersRequestModel_ToGetResourceServersQuery()
    {
        var input = new GetResourceServersRequestModel(
            Cursor: 1234L,
            PageSize: 100,
            Name: "name",
            Audience: "audience",
            CreatedFrom: DateTime.UnixEpoch.AddYears(1),
            CreatedTo: DateTime.UnixEpoch.AddYears(2),
            ModifiedFrom: DateTime.UnixEpoch.AddYears(3),
            ModifiedTo: DateTime.UnixEpoch.AddYears(4));

        var actual = _mapper.Map<GetResourceServersQuery>(input);
        actual.Cursor.Should().Be(input.Cursor);
        actual.PageSize.Should().Be(input.PageSize);
        actual.Name.Should().Be(input.Name);
        actual.Audience.Should().Be(input.Audience);
        actual.CreatedFrom.Should().Be(input.CreatedFrom);
        actual.CreatedTo.Should().Be(input.CreatedTo);
        actual.ModifiedFrom.Should().Be(input.ModifiedFrom);
        actual.ModifiedTo.Should().Be(input.ModifiedTo);
    }
    
    /*
     * ResourceServer => ResourceServerModel
     */
    
    [Fact]
    public void ResourceServersProfile_MapsResourceServer_ToResourceServerModel()
    {
        var baseDate = new DateTimeOffset(2024, 12, 2, 9, 48, 37, TimeSpan.Zero);
        var entity = GetEntity(baseDate);
        var actual = _mapper.Map<ResourceServerModel>(entity);
        actual.Id.Should().Be(entity.Id);
        actual.Name.Should().Be(entity.Name);
        actual.Audience.Should().Be(entity.Audience);
        actual.DateCreated.Should().Be(entity.DateCreated);
        actual.DateModified.Should().Be(entity.DateModified);
    }
    
    /*
     * IEnumerable<ResourceServer> => ResultPage<ResourceServerModel>
     */
    
    [Fact]
    public void ResourceServersProfile_MapsResourceServerCollection_ToResourceServerModelPage()
    {
        var baseDate = new DateTimeOffset(2024, 12, 2, 9, 48, 37, TimeSpan.Zero);
        
        var entities = new[]
        {
            GetEntity(baseDate.AddHours(1)),
            GetEntity(baseDate.AddHours(2)),
            GetEntity(baseDate.AddHours(3))
        };

        // We've already tested this mapping, so we can trust the inner collection
        var expectedData = entities.Select(entity => _mapper.Map<ResourceServerModel>(entity));
        var expectedCursor = entities.GetMaximumCursor();
        
        var actual = _mapper.Map<ResultPage<ResourceServerModel>>(entities);
        actual.Data.Should().BeEquivalentTo(expectedData);
        actual.Cursor.Should().Be(expectedCursor);
        actual.Count.Should().Be(3);
    }
    
    /*
     * ResourceServer => ResourceServerDetailsModel
     */
    
    [Fact]
    public void ResourceServersProfile_MapsResourceServer_ToResourceServerDetailsModel()
    {
        var baseDate = new DateTimeOffset(2024, 12, 2, 9, 48, 37, TimeSpan.Zero);
        var entity = GetEntity(baseDate);
        var expectedScopes = entity.Scopes.Select(scope 
            => new ResourceServerScopeModel(scope.Value, scope.Description));
        
        var actual = _mapper.Map<ResourceServerDetailsModel>(entity);
        actual.Id.Should().Be(entity.Id);
        actual.Name.Should().Be(entity.Name);
        actual.Audience.Should().Be(entity.Audience);
        actual.TokenLifetime.Should().Be(entity.TokenLifetime);
        actual.Scopes.Should().BeEquivalentTo(expectedScopes);
        actual.DateCreated.Should().Be(entity.DateCreated);
        actual.DateModified.Should().Be(entity.DateModified);
    }
    
    /*
     * CreateResourceServerRequestModel => CreateResourceServerCommand
     */

    [Fact]
    public void ResourceServersProfile_MapsCreateResourceServerRequestModel_ToCreateResourceServerCommand()
    {
        var model = new CreateResourceServerRequestModel(
            Name: "name",
            Audience: "audience",
            TokenLifetime: 7,
            Scopes:
            [
                new ResourceServerScopeModel("scope.one", "description of scope.one"),
                new ResourceServerScopeModel("scope.two", "description of scope.two"),
                new ResourceServerScopeModel("scope.three", "description of scope.three")
            ]);

        var expected = new CreateResourceServerCommand(
            Name: model.Name, 
            Audience: model.Audience, 
            TokenLifetime: model.TokenLifetime,
            Scopes: model.Scopes.Select(scope => new RequestScopeModel(scope.Value, scope.Description)).ToList());

        var actual = _mapper.Map<CreateResourceServerCommand>(model);
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * Private methods
     */
    
    private static ResourceServer GetEntity(
        DateTimeOffset? baseDate, 
        string name = "name",
        string audience = "audience",
        string externalId = "external-id",
        int tokenLifetime = 43200)
    {
        using var dateContext = new DateTimeOffsetProviderContext(baseDate ?? DateTimeOffset.UnixEpoch);
        return new ResourceServer(externalId, audience, name, tokenLifetime)
        {
            Scopes = [
                new Scope("", "scope.one", "description of scope one"),
                new Scope("", "scope.two", "description of scope two"),
                new Scope("", "scope.three", "description of scope three")
            ],
            DateModified = (baseDate ?? DateTimeOffset.UnixEpoch).UtcDateTime.AddYears(1)
        };
    }
}