using FluentValidation.Results;
using Reapit.Platform.Products.Core.UseCases.ProductClients.PatchProductClient;
using Reapit.Platform.Products.Data.Repositories.ProductClients;
using Reapit.Platform.Products.Data.Services;
using Reapit.Platform.Products.Domain.Entities;
using static Reapit.Platform.Products.Core.UnitTests.UseCases.ProductClients.TestProductClientFactory;

namespace Reapit.Platform.Products.Core.UnitTests.UseCases.ProductClients.PatchProductClient;

public class PatchProductClientCommandHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IProductClientRepository _repository = Substitute.For<IProductClientRepository>();
    private readonly IValidator<PatchProductClientCommand> _validator = Substitute.For<IValidator<PatchProductClientCommand>>();
    private readonly FakeLogger<PatchProductClientCommandHandler> _logger = new();

    /*
     * Handle
     */

    [Fact]
    public async Task Handle_ThrowsValidationException_WhenValidationFailed()
    {
        ConfigureValidation(false);

        var request = GetRequest();
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<ValidationException>();
    }
    
    [Fact]
    public async Task Handle_ThrowNotFoundException_WhenProductNotFound()
    {
        const string id = "test-id";
        
        ConfigureValidation(true);
    
        _repository.GetProductClientByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<ProductClient?>(null));
    
        var request = GetRequest(id: id);
        var sut = CreateSut();
        var action = () => sut.Handle(request, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task Handle_ReturnsWithoutUpdate_WhenEntityUnchanged()
    {
        var client = GetProductClient(
            callbackUrls: [ "https://example.net/callback" ], 
            signOutUrls: [ "https://example.net/sign-out" ]);
        
        ConfigureValidation(true);
    
        _repository.GetProductClientByIdAsync(client.Id, Arg.Any<CancellationToken>())
            .Returns(client);
    
        var request = GetRequest(client.Id, client.Name, client.Description, client.CallbackUrls, client.SignOutUrls);
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeSameAs(client);
    
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<ProductClient>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Handle_ReturnsAfterUpdate_WhenChanged()
    {
        var client = GetProductClient();
        
        ConfigureValidation(true);
    
        _repository.GetProductClientByIdAsync(client.Id, Arg.Any<CancellationToken>())
            .Returns(client);
    
        var request = GetRequest(
            id: client.Id, 
            name: "new name", 
            description: "new description",
            callbackUrls: [ "https://example.net/callback" ],
            signOutUrls: [ "https://example.net/sign-out" ]);
        
        var sut = CreateSut();
        var actual = await sut.Handle(request, default);
        actual.Should().BeSameAs(client);
    
        await _repository.Received(1).UpdateAsync(Arg.Is<ProductClient>(productParam 
            => productParam.Name == request.Name 
               && productParam.Description == request.Description
               && productParam.CallbackUrls == request.CallbackUrls
               && productParam.SignOutUrls == request.SignOutUrls
            ), Arg.Any<CancellationToken>());
        
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
    
    /*
     * Private methods
     */

    private PatchProductClientCommandHandler CreateSut()
    {
        _unitOfWork.ProductClients.Returns(_repository);
        return new PatchProductClientCommandHandler(_unitOfWork, _validator, _logger);
    }

    private static PatchProductClientCommand GetRequest(
        string id = "id", 
        string? name = "name", 
        string? description = "description",
        ICollection<string>? callbackUrls = null,
        ICollection<string>? signOutUrls = null)
        => new(id, name, description, callbackUrls, signOutUrls);

    private void ConfigureValidation(bool isSuccessful)
    {
        var result =  isSuccessful
            ? new ValidationResult()
            : new ValidationResult([new ValidationFailure("propertyName", "errorMessage")]);

        _validator.ValidateAsync(Arg.Any<PatchProductClientCommand>(), Arg.Any<CancellationToken>())
            .Returns(result);
    }
}