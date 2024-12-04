using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Api.Controllers.Grants.V1;
using Reapit.Platform.Products.Api.Controllers.Grants.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Api.Extensions;
using Reapit.Platform.Products.Core.UseCases.Grants.CreateGrant;
using Reapit.Platform.Products.Core.UseCases.Grants.GetGrants;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Api.UnitTests.Controllers.Grants.V1;

public class GrantsProfileTests
{
    private readonly IMapper _mapper = new MapperConfiguration(cfg 
            => cfg.AddProfile<GrantsProfile>())
        .CreateMapper();
    
    /*
     * Grant => GrantModel
     * Client => GrantClientModel
     * ResourceServer => GrantResourceServerModel
     */

    [Fact]
    public void GrantsProfile_MapsEntity_ToGrantModel()
    {
        var scopes = new[] { "abc.def", "123.456", "hey.macarena" };
        var entity = GetEntity(scopes: scopes);
        var expected = new GrantModel(
            Id: entity.Id,
            DateCreated: entity.DateCreated,
            DateModified: entity.DateModified,
            Scopes: scopes,
            Client: new GrantClientModel(entity.Client.Id, entity.Client.Name, entity.Client.Type.Name),
            ResourceServer: new GrantResourceServerModel(entity.ResourceServer.Id, entity.ResourceServer.Name));

        var actual = _mapper.Map<GrantModel>(entity);
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * IEnumerable<Grant> => ResultPage<GrantModel>
     */

    [Fact]
    public void GrantsProfile_MapsEntityCollection_ToResultPage()
    {
        var baseDate = new DateTimeOffset(2024, 12, 2, 9, 48, 37, TimeSpan.Zero);
        
        var entities = new[]
        {
            GetEntity(clientName: "client one", clientType: ClientType.AuthCode, resourceServerName: "rs one", baseDate: baseDate.AddHours(1), scopes: ["one"]),
            GetEntity(clientName: "client two", clientType: ClientType.AuthCode, resourceServerName: "rs two", baseDate: baseDate.AddHours(2), scopes: ["two"]),
            GetEntity(clientName: "client three", clientType: ClientType.AuthCode, resourceServerName: "rs three", baseDate: baseDate.AddHours(3), scopes: ["three"])
        };

        // We've already tested this mapping, so we can trust the inner collection
        var expectedData = entities.Select(entity => _mapper.Map<GrantModel>(entity));
        var expectedCursor = entities.GetMaximumCursor();
        
        var actual = _mapper.Map<ResultPage<GrantModel>>(entities);
        actual.Data.Should().BeEquivalentTo(expectedData);
        actual.Cursor.Should().Be(expectedCursor);
        actual.Count.Should().Be(3);
    }
    
    /*
     * GetGrantsRequestModel => GetGrantsQuery
     */
    
    [Fact]
    public void GrantsProfile_MapsGetGrantsRequestModel_ToGetGrantsQuery()
    {
        var baseDate = new DateTime(2024, 12, 2, 9, 48, 37, DateTimeKind.Utc);
        var input = new GetGrantsRequestModel(
            Cursor: 1234L,
            PageSize: 72,
            ClientId: "client-id",
            ResourceServerId: "resource-server-id",
            CreatedFrom: baseDate.AddDays(1),
            CreatedTo: baseDate.AddDays(2),
            ModifiedFrom: baseDate.AddDays(3),
            ModifiedTo: baseDate.AddDays(4));

        var expected = new GetGrantsQuery(
            input.Cursor, 
            input.PageSize, 
            input.ClientId, 
            input.ResourceServerId,
            input.CreatedFrom, 
            input.CreatedTo, 
            input.ModifiedFrom, 
            input.ModifiedTo);

        var actual = _mapper.Map<GetGrantsQuery>(input);
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * CreateGrantRequestModel => CreateGrantCommand
     */

    [Fact]
    public void GrantsProfile_MapsCreateGrantRequestModel_ToCreateGrantCommand()
    {
        var input = new CreateGrantRequestModel("client-id", "resource-server-id", ["scope.one", "scope.two", "scope.three"]);
        var expected = new CreateGrantCommand(input.ClientId, input.ResourceServerId, input.Scopes);
        var actual = _mapper.Map<CreateGrantCommand>(input);
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * Private methods
     */

    private static Grant GetEntity(
        string clientName = "client name",
        string clientType = "machine",
        string resourceServerName = "resource server name",
        ICollection<string>? scopes = null,
        DateTimeOffset? baseDate = null)
    {
        baseDate ??= DateTimeOffset.UnixEpoch;
        using var _ = new DateTimeOffsetProviderContext(baseDate.Value);
        
        var client = GetClient(clientName, clientType);
        var resourceServer = GetResourceServer(resourceServerName);
        return new Grant(
            externalId: "never exposed",
            clientId: client.Id,
            resourceServerId: resourceServer.Id)
        {
            Client = client,
            ResourceServer = resourceServer,
            Scopes = (scopes ?? []).Select(scope => new Scope(resourceServer.Id, scope, null)).ToList(),
            DateModified = baseDate.Value.AddYears(1).UtcDateTime
        };
    }
    
    private static Client GetClient(string name, ClientType? type = null)
        => new("", "", type ?? ClientType.Machine,  name, null, null, null, null);

    private static ResourceServer GetResourceServer(string name) 
        => new("", "", name, 3600);
}