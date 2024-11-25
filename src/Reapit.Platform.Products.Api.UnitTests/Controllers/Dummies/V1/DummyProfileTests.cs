using Reapit.Platform.Products.Api.Controllers.Dummies.V1;
using Reapit.Platform.Products.Api.Controllers.Dummies.V1.Models;
using Reapit.Platform.Products.Core.UseCases.Dummies.CreateDummy;
using Reapit.Platform.Products.Core.UseCases.Dummies.GetDummies;
using Reapit.Platform.Products.Domain.Entities;

namespace Reapit.Platform.Products.Api.UnitTests.Controllers.Dummies.V1;

public class DummyProfileTests
{
    /*
     * ReadDummiesRequestModel => GetDummiesQuery
     */

    [Fact]
    public void DummyProfile_MapsReadDummiesRequestModel_ToGetDummiesQuery_WhenParametersNull()
    {
        var input = new ReadDummiesRequestModel(null, null, null, null, null);
        var expected = new GetDummiesQuery(null, null, null, null, null);

        var sut = CreateSut();
        var actual = sut.Map<GetDummiesQuery>(input);
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void DummyProfile_MapsReadDummiesRequestModel_ToGetDummiesQuery_WhenParametersSet()
    {
        var input = new ReadDummiesRequestModel(
            Name: "test-name", 
            CreatedFrom: DateTime.UnixEpoch, 
            CreatedTo: DateTime.UnixEpoch.AddDays(1), 
            ModifiedFrom: DateTime.UnixEpoch.AddDays(2), 
            ModifiedTo: DateTime.UnixEpoch.AddDays(3));
        
        var expected = new GetDummiesQuery(input.Name, input.CreatedFrom, input.CreatedTo, input.ModifiedFrom, input.ModifiedTo);

        var sut = CreateSut();
        var actual = sut.Map<GetDummiesQuery>(input);
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * WriteDummyRequestModel => CreateDummyCommand
     */

    [Fact]
    public void DummyProfile_MapsWriteDummyRequestModel_ToCreateDummyCommand()
    {
        var input = new WriteDummyRequestModel("test-name");
        var expected = new CreateDummyCommand(input.Name);

        var sut = CreateSut();
        var actual = sut.Map<CreateDummyCommand>(input);
        actual.Should().BeEquivalentTo(expected);
    }
    
    /*
     * Dummy => ReadDummyResponseModel
     */

    [Fact]
    public void DummyProfile_MapsReadDummiesRequestModel_ToGetDummiesQuery()
    {
        var input = new Dummy("this dummy") { Id = Guid.NewGuid() };
        var expected = new ReadDummyResponseModel($"{input.Id:N}", input.Name, input.DateCreated, input.DateModified);

        var sut = CreateSut();
        var actual = sut.Map<ReadDummyResponseModel>(input);
        actual.Should().BeEquivalentTo(expected);
    }

    /*
     * Private methods
     */
    
    private static IMapper CreateSut() 
        => new MapperConfiguration(cfg => cfg.AddProfile(typeof(DummyProfile))).CreateMapper();
}