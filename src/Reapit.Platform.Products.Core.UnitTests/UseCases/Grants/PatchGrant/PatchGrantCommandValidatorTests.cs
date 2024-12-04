using Reapit.Platform.Products.Core.UseCases.Grants;
using Reapit.Platform.Products.Core.UseCases.Grants.PatchGrant;
using Reapit.Platform.Products.Data.Repositories.Grants;
using Reapit.Platform.Products.Data.Repositories.ResourceServers;
using Reapit.Platform.Products.Data.Services;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.Grants.PatchGrant;

public class PatchGrantCommandValidatorTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IGrantRepository _grantRepository = Substitute.For<IGrantRepository>();
    private readonly IResourceServerRepository _resourceServerRepository = Substitute.For<IResourceServerRepository>();

    [Fact]
    public async Task Validate_ReturnsSuccess_WhenRequestValid()
    {
        var request = new PatchGrantCommand("id", ["scope.three", "scope.four"]);

        var grant = GetGrant();
        _grantRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(grant);

        _resourceServerRepository.GetByIdAsync(grant.ResourceServerId, Arg.Any<CancellationToken>())
            .Returns(GetResourceServer("scope.one", "scope.two", "scope.three", "scope.four"));

        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Pass();
    }
    
    [Fact]
    public async Task Validate_ReturnsSuccess_WhenNoGrantFound()
    {
        var request = new PatchGrantCommand("id", ["scope.three", "scope.four"]);
        _grantRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Entities.Grant?>(null));

        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Pass();
    }
    
    [Fact]
    public async Task Validate_ReturnsSuccess_WhenNoResourceServerFound()
    {
        var request = new PatchGrantCommand("id", ["scope.three", "scope.four"]);
        var grant = GetGrant();
        _grantRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(grant);
        
        _resourceServerRepository.GetByIdAsync(grant.ResourceServerId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Entities.ResourceServer?>(null));

        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Pass();
    }
    
    [Fact]
    public async Task Validate_ReturnsSuccess_WhenNoScopesRequested_AndNoScopesAvailable()
    {
        var request = new PatchGrantCommand("id", []);
        var grant = GetGrant();
        
        _grantRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(grant);

        _resourceServerRepository.GetByIdAsync(grant.ResourceServerId, Arg.Any<CancellationToken>())
            .Returns(GetResourceServer());
        
        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Pass();
    }
    
    [Fact]
    public async Task Validate_ReturnsFailure_WhenRequestedScopeNotAvailable()
    {
        var request = new PatchGrantCommand("id", ["scope.three", "scope.five", "scope.four"]);
        var grant = GetGrant();
        _grantRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(grant);

        _resourceServerRepository.GetByIdAsync(grant.ResourceServerId, Arg.Any<CancellationToken>())
            .Returns(GetResourceServer("scope.one", "scope.two", "scope.three", "scope.four"));

        var sut = CreateSut();
        var result = await sut.ValidateAsync(request);
        result.Should().Fail("Scopes[1]", GrantValidationMessages.UnsupportedScope);
    }
    
    /*
     * Private methods
     */

    private PatchGrantCommandValidator CreateSut()
    {
        _unitOfWork.Grants.Returns(_grantRepository);
        _unitOfWork.ResourceServers.Returns(_resourceServerRepository);
        return new PatchGrantCommandValidator(_unitOfWork);
    }

    private static Entities.Grant GetGrant()
        => new("", "", "resource-server-id")
        {
            Client = default!,
            ResourceServer = default!
        };
    
    private static Entities.ResourceServer GetResourceServer(params string[]? scopes)
        => new("", "", "", 1)
        {
            Scopes = (scopes ?? []).Select(scope => new Entities.Scope("", scope, "")).ToList()
        };
}