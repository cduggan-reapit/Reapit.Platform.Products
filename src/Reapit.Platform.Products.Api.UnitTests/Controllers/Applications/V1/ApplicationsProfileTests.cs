using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Products.Api.Controllers.Applications.V1;
using Reapit.Platform.Products.Api.Controllers.Applications.V1.Models;
using Reapit.Platform.Products.Api.Controllers.Shared;
using Reapit.Platform.Products.Api.Extensions;
using Reapit.Platform.Products.Core.UseCases.Applications.CreateApplication;
using Reapit.Platform.Products.Core.UseCases.Applications.GetApplications;
using Reapit.Platform.Products.Domain.Entities;
using Reapit.Platform.Products.Domain.Entities.Enums;

namespace Reapit.Platform.Products.Api.UnitTests.Controllers.Applications.V1;

public class ApplicationsProfileTests
{
    private readonly IMapper _mapper = new MapperConfiguration(cfg 
            => cfg.AddProfile<ApplicationsProfile>())
        .CreateMapper();
    
    /*
     * App => ApplicationModel
     */

    [Fact]
    public void ApplicationsProfile_MapsApp_ToApplicationModel()
    {
        var app = GetEntity();
        var expected = new ApplicationModel(app.Id, app.Name, app.IsFirstParty, app.DateCreated, app.DateModified);
        var actual = _mapper.Map<ApplicationModel>(app);
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * IEnumerable<App> => ResultPage<ApplicationModel>
     */
    
    [Fact]
    public void ApplicationsProfileProfile_MapsAppCollection_ToModelPage()
    {
        var baseDate = new DateTimeOffset(2024, 12, 2, 9, 48, 37, TimeSpan.Zero);
        
        var entities = new[]
        {
            GetEntity(baseDate.AddHours(1)),
            GetEntity(baseDate.AddHours(2)),
            GetEntity(baseDate.AddHours(3))
        };

        // We've already tested this mapping, so we can trust the inner collection
        var expectedData = entities.Select(entity => _mapper.Map<ApplicationModel>(entity));
        var expectedCursor = entities.GetMaximumCursor();
        
        var actual = _mapper.Map<ResultPage<ApplicationModel>>(entities);
        actual.Data.Should().BeEquivalentTo(expectedData);
        actual.Cursor.Should().Be(expectedCursor);
        actual.Count.Should().Be(3);
    }
    
    /*
     * App => ApplicationDetailsModel
     * Client => ApplicationClientModel
     */

    [Fact]
    public void ApplicationsProfile_MapsApp_ToApplicationDetailsModel_WithNestedApplicationClientModel()
    {
        var app = GetEntity();
        var clients = app.Clients.Select(client => new ApplicationClientModel(client.Id, client.Name, client.Type.Name));
        var expected = new ApplicationDetailsModel(app.Id, app.Name, app.Description, app.IsFirstParty, app.DateCreated, app.DateModified, clients);
        var actual = _mapper.Map<ApplicationDetailsModel>(app);
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * CreateApplicationRequestModel => CreateApplicationCommand
     */

    [Fact]
    public void ApplicationsProfile_MapsCreateApplicationRequestModel_ToCreateApplicationCommand()
    {
        var model = new CreateApplicationRequestModel("name", "description", true);
        var expected = new CreateApplicationCommand(model.Name, model.Description, model.IsFirstParty);
        var actual = _mapper.Map<CreateApplicationCommand>(model);
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * GetApplicationsRequestModel => GetApplicationsQuery
     */

    [Fact]
    public void ApplicationsProfile_MapsGetApplicationsRequestModel_ToGetApplicationsQuery()
    {
        var model = new GetApplicationsRequestModel(
            Cursor: 1234L, 
            PageSize: 101, 
            Name: "name", 
            Description: "description", 
            IsFirstParty: true,
            CreatedFrom: DateTime.UnixEpoch.AddYears(1),
            CreatedTo: DateTime.UnixEpoch.AddYears(2),
            ModifiedFrom: DateTime.UnixEpoch.AddYears(3),
            ModifiedTo: DateTime.UnixEpoch.AddYears(4));
        
        var expected = new GetApplicationsQuery(
            Cursor: model.Cursor, 
            PageSize: model.PageSize, 
            Name: model.Name, 
            Description: model.Description, 
            IsFirstParty: model.IsFirstParty, 
            CreatedFrom: model.CreatedFrom, 
            CreatedTo: model.CreatedTo, 
            ModifiedFrom: model.ModifiedFrom, 
            ModifiedTo: model.ModifiedTo);
        
        var actual = _mapper.Map<GetApplicationsQuery>(model);
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * Private methods
     */
    
    private static App GetEntity(
        DateTimeOffset? baseDate = null, 
        string name = "name",
        string description = "description",
        bool isFirstParty = false)
    {
        using var dateContext = new DateTimeOffsetProviderContext(baseDate ?? DateTimeOffset.UnixEpoch);
        var app = new App(name, description, isFirstParty)
        {
            DateModified = (baseDate ?? DateTimeOffset.UnixEpoch).UtcDateTime.AddYears(1)
        };
        app.Clients.Add(new Client(app.Id, "external-id", ClientType.Machine, "m2m client", null, null, null, null));
        app.Clients.Add(new Client(app.Id, "external-id", ClientType.AuthCode, "authorization code client", null, null, null, null));
        return app;
    }
}