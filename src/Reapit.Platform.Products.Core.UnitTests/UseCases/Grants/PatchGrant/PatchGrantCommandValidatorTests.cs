using Reapit.Platform.Products.Core.UseCases.Grants;
using Reapit.Platform.Products.Core.UseCases.Grants.PatchGrant;
using Reapit.Platform.Products.Data.Repositories.Grants;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Grants.PatchGrant;

public class PatchGrantCommandValidatorTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IGrantRepository _repository = Substitute.For<IGrantRepository>();

    [Fact]
    public async Task Validate_ReturnsSuccess_WhenRequestValid()
    {
        var request = new PatchGrantCommand("id", ["scope.three", "scope.four"]);
        _repository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(GetGrant("scope.one", "scope.two", "scope.three", "scope.four"));

        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Pass();
    }
    
    [Fact]
    public async Task Validate_ReturnsSuccess_WhenNoGrantFound()
    {
        var request = new PatchGrantCommand("id", ["scope.three", "scope.four"]);
        _repository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Entities.Grant?>(null));

        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Pass();
    }
    
    [Fact]
    public async Task Validate_ReturnsSuccess_WhenNoScopesRequested_AndNoScopesAvailable()
    {
        var request = new PatchGrantCommand("id", []);
        _repository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(GetGrant());

        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Pass();
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenRequestScopeNotAvailable()
    {
        var request = new PatchGrantCommand("id", ["scope.three", "scope.five", "scope.four"]);
        _repository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(GetGrant("scope.one", "scope.two", "scope.three", "scope.four"));

        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail("Scopes[1]", GrantValidationMessages.UnsupportedScope);
    }
    
    /*
     * Private methods
     */

    private PatchGrantCommandValidator CreateSut()
    {
        _unitOfWork.Grants.Returns(_repository);
        return new PatchGrantCommandValidator(_unitOfWork);
    }

    private static Entities.Grant GetGrant(params string[]? scopes)
        => new Entities.Grant("", "", "")
        {
            ResourceServer = GetResourceServer(scopes)
        };
    
    private static Entities.ResourceServer GetResourceServer(params string[]? scopes)
        => new("", "", "", 1)
        {
            Scopes = (scopes ?? []).Select(scope => new Entities.Scope("", scope, "")).ToList()
        };
}