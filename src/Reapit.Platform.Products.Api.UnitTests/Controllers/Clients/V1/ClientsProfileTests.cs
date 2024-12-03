using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Api.Controllers.Clients.V1;
using Reapit.Platform.Products.Api.Controllers.Clients.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Api.Extensions;
using Reapit.Platform.Products.Core.UseCases.Clients.CreateClient;
using Reapit.Platform.Products.Core.UseCases.Clients.GetClients;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Api.UnitTests.Controllers.Clients.V1;

public class ClientsProfileTests
{
    private readonly IMapper _mapper = new MapperConfiguration(cfg 
            => cfg.AddProfile<ClientsProfile>())
        .CreateMapper();

    /*
     *  GetClientsRequestModel => GetClientsQuery
    */
    
    [Fact]
    public void ClientsProfile_MapsGetClientsRequestModel_ToGetClientsQuery()
    {
        var baseDate = new DateTime(2020, 1, 1, 12, 32, 17, DateTimeKind.Utc);
        var input = new GetClientsRequestModel(
            Cursor: 1L,
            PageSize: 30,
            AppId: "appId",
            Type: ClientType.Machine.Name,
            Name: "name",
            Description: "description",
            CreatedFrom: baseDate,
            CreatedTo: baseDate.AddDays(1),
            ModifiedFrom: baseDate.AddDays(2),
            ModifiedTo: baseDate.AddDays(3));

        var expected = new GetClientsQuery(
            Cursor: input.Cursor, 
            PageSize: input.PageSize, 
            AppId: input.AppId, 
            Type: input.Type, 
            Name: input.Name,
            Description: input.Description, 
            CreatedFrom: input.CreatedFrom, 
            CreatedTo: input.CreatedTo, 
            ModifiedFrom: input.ModifiedFrom, 
            ModifiedTo: input.ModifiedTo);

        var actual = _mapper.Map<GetClientsQuery>(input);
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     *  Client => ClientModel
     */

    [Fact]
    public void ClientsProfile_MapsClient_ToClientModel()
    {
        var entity = GetEntity(null);
        var expected = new ClientModel(entity.Id, entity.AppId, entity.Type.Name, entity.Name, entity.DateCreated, entity.DateModified);
        var actual = _mapper.Map<ClientModel>(entity);
        actual.Should().BeEquivalentTo(expected);
    }

    /*
     * IEnumerable<Client> => ResultPage<ClientModel>
     */

    [Fact]
    public void ClientsProfile_MapsClientCollection_ToResultPage()
    {
        var baseDate = new DateTimeOffset(2024, 12, 2, 9, 48, 37, TimeSpan.Zero);
        
        var entities = new[]
        {
            GetEntity(baseDate.AddHours(1)),
            GetEntity(baseDate.AddHours(2)),
            GetEntity(baseDate.AddHours(3))
        };

        // We've already tested this mapping, so we can trust the inner collection
        var expectedData = entities.Select(entity => _mapper.Map<ClientModel>(entity));
        var expectedCursor = entities.GetMaximumCursor();
        
        var actual = _mapper.Map<ResultPage<ClientModel>>(entities);
        actual.Data.Should().BeEquivalentTo(expectedData);
        actual.Cursor.Should().Be(expectedCursor);
        actual.Count.Should().Be(3);
    }
    
    /*
     * Client => ClientDetailsModel
     */

    [Fact]
    public void ClientsProfile_MapsClient_ToClientDetailsModel()
    {
        var entity = GetEntity(null);

        var expectedGrants = entity.Grants.Select(grant 
                => new ClientGrantModel(grant.Id, grant.ResourceServerId, grant.ResourceServer?.Name))
            .ToList();

        var expected = new ClientDetailsModel(
            Id: entity.Id,
            AppId: entity.AppId,
            Type: entity.Type.Name,
            Name: entity.Name,
            Description: entity.Description,
            LoginUrl: entity.LoginUrl,
            CallbackUrls: entity.CallbackUrls,
            SignOutUrls: entity.SignOutUrls,
            DateCreated: entity.DateCreated,
            DateModified: entity.DateModified,
            Grants: expectedGrants);

        var actual = _mapper.Map<ClientDetailsModel>(entity);
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * CreateClientRequestModel => CreateClientCommand
     */

    [Fact]
    public void ClientsProfile_MapsCreateClientRequestModel_ToCreateClientCommand()
    {
        var input = new CreateClientRequestModel(
            AppId: "app-id",
            Type: "machine",
            Name: "test name",
            Description: "description",
            LoginUrl: "login-url",
            CallbackUrls: ["callback-url"],
            SignOutUrls: ["sign-out-url"]);

        var expected = new CreateClientCommand(
            AppId: input.AppId,
            Type: input.Type,
            Name: input.Name,
            Description: input.Description,
            LoginUrl: input.LoginUrl,
            CallbackUrls: input.CallbackUrls,
            SignOutUrls: input.SignOutUrls);

        var actual = _mapper.Map<CreateClientCommand>(input);
        actual.Should().BeEquivalentTo(expected);
    }

    /*
     * Private methods
     */
    
    private static Client GetEntity(DateTimeOffset? baseDate)
    {
        using var dateContext = new DateTimeOffsetProviderContext(baseDate ?? DateTimeOffset.UnixEpoch);
        var client = new Client(
            appId: "app-id", 
            externalId: "external-id", 
            type: ClientType.AuthCode, 
            name: "auth code client", 
            description: "description of an auth code client", 
            loginUrl: "https://example.com/login",
            callbackUrls: [ "https://example.com/callback" ],
            signOutUrls: [ "https://example.com/signOut" ]) 
        {
            DateModified = (baseDate ?? DateTimeOffset.UnixEpoch).UtcDateTime.AddYears(1),
        };
        
        client.Grants.Add(GetGrant(client.Id, "resource one"));
        client.Grants.Add(GetGrant(client.Id, "resource two"));
        client.Grants.Add(GetGrant(client.Id, "resource three", false));
        return client;
    }

    private static Grant GetGrant(string clientId, string resourceServerName, bool includeMapping = true)
    {
        var resourceServer = GetResourceServer(resourceServerName);
        
        var guid = Guid.NewGuid();
        using var _ = new GuidProviderContext(guid);
        var grant = new Grant("external-id-1", clientId, resourceServer.Id);
        if (includeMapping)
            grant.ResourceServer = resourceServer;

        return grant;
    }

    private static ResourceServer GetResourceServer(string name)
    {
        using var _ = new GuidProviderContext(Guid.NewGuid());
        return new ResourceServer("", "", name, 3600);
    }
}